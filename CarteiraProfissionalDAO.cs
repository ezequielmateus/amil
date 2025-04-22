using Amil.SisMed.DTO;
using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amil.SisMed.DAL
{
    public class CarteiraProfissionalDAO : BaseDAO
    {
        internal string _mensagemErro = "Ocorreu um erro durante a operação : {0} , detalhes do erro : {1}";

        /// <summary>
        /// Obtem lista de profissional por tipo de carteira e equipe de saude
        /// </summary>
        public List<CarteiraProfissionalDTO> ListarCarteirasProfissionais(int tipoCarteiraID, int? equipeSaudeID)
        {
            return ObterListagemCarteirasProfissionais(tipoCarteiraID, equipeSaudeID, null);
        }

        /// <summary>
        /// Obtem lista de profissionais por tipo de carteira, equipe de saude + filtro por tipo de papel
        /// </summary>
        public List<CarteiraProfissionalDTO> ListarCarteirasProfissionais(int tipoCarteiraID, int? equipeSaudeID, string tipoPapelProfissional)
        {
            return ObterListagemCarteirasProfissionais(tipoCarteiraID, equipeSaudeID, tipoPapelProfissional);
        }

        private List<CarteiraProfissionalDTO> ObterListagemCarteirasProfissionais(int tipoCarteiraID, int? equipeSaudeID, string tipoPapelProfissional)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@"
                        SELECT 
                            P.ID_PROFISSIONAL as ProfissionalID, 
                            P.NOME as ProfissionalNome,
                            P.ID_SITUACAO as SituacaoID,
                            P.ID_CONSELHO as ConselhoID,
                            C.SIGLA as Sigla,
                            P.NUM_CONSELHO as ProfissionalNumeroConselho,
                            CP.ID_PROFISSIONAL_SUBSTITUTO as ProfissionalSubstitutoID,
                            (CASE WHEN CP.ID_PROFISSIONAL_SUBSTITUTO IS NOT NULL THEN PS.NOME ELSE '' END) AS ProfissionalSubstitutoNome, 
                            (CASE WHEN CP.ID_PROFISSIONAL_SUBSTITUTO IS NOT NULL THEN PS.NUM_CONSELHO ELSE '' END) AS NumeroConselhoSubstituto,
                            CP.ID_TIPOCARTEIRA as TipoCarteiraID,
                            CP.TP_PAPEL as TipoPapel,
                            CP.VL_LIMITE as PontuacaoProfissionalLimite,
                            (
                                CASE WHEN (TP.TP_LIMITE = 'P') THEN (
                                                                  SELECT
                                                                      NVL (SUM(
                                                                          CASE MD.FL_STANDBY WHEN 1 THEN 0
                                                                                             WHEN 4 THEN 0
                                                                                             ELSE ( 
                                                                                                    CASE WHEN (MD.ID_PESSOA IS NOT NULL) THEN 
                                                                                                    (SELECT CCP.VL_FAIXA 
                                                                                                     FROM AMESP.GLB_PESSOA_FISICA PF 
                                                                                                     INNER JOIN AMESP.CONFIG_CARTEIRA_PONT CCP ON (CCP.FL_GENERO = PF.ID_SEXO AND AMESP.CALC_IDADE(PF.DT_NASCIMENTO, SYSDATE) BETWEEN CCP.VL_IDADE_INI AND CCP.VL_IDADE_FIM) 
                                                                                                     WHERE PF.ID_PESSOA = MD.ID_PESSOA
                                                                                                     ) ELSE 0 END 
                                                                                                   ) END 
                                                                          ), 0)  LIMITEQUANTIDADETOTAL 
                                                                  FROM AMESP.GCP_CARTEIRA_PESSOA MD 
                                                                  WHERE MD.ID_PROFISSIONAL = CP.ID_PROFISSIONAL
                                                                    AND MD.ID_TIPOCARTEIRA = CP.ID_TIPOCARTEIRA
                                                                    AND MD.ID_EQUIPE = CP.ID_EQUIPE
                                                                    AND MD.TP_PAPEL = CP.TP_PAPEL
                                                                    
                                                                    )
                                ELSE CP.QTDE_ATUAL_CARTEIRA
                                END
                            ) AS PontuacaoProfissionalAtual,
                            CP.ID_EQUIPE as EquipeSaudeID,
                            CP.FL_RECEBE_APRAZ as PermiteAprazamento,
                            TP.FL_ATIVA_LIMITE as LimiteAtivo,
                            TP.TP_LIMITE AS TipoLimite
                        FROM AMESP.GCP_CARTEIRA_PROFISSIONAL CP
                            INNER JOIN AMESP.PROFISSIONAL_SAUDE P ON P.id_profissional = CP.ID_PROFISSIONAL
                            INNER JOIN AMESP.GCP_TIPOCARTEIRA TP ON TP.ID_TIPOCARTEIRA = CP.ID_TIPOCARTEIRA 
                            INNER JOIN AMESP.CONSELHO_PROFISSIONAL C ON P.ID_CONSELHO = C.ID_CONSELHO
                            LEFT JOIN AMESP.PROFISSIONAL_SAUDE PS ON PS.id_profissional = CP.ID_PROFISSIONAL_SUBSTITUTO
                        WHERE 
                            CP.ID_TIPOCARTEIRA = :TipoCarteiraID ");

            parameters.Add(new OracleParameter("TipoCarteiraID", tipoCarteiraID));

            if (equipeSaudeID.HasValue && equipeSaudeID.Value > 0)
            {
                query.Append(" AND CP.ID_EQUIPE = :EquipeSaudeID ");
                parameters.Add(new OracleParameter("EquipeSaudeID", equipeSaudeID));
            }
            else
            {
                query.Append(" AND CP.ID_EQUIPE IS NULL ");
            }

            if (!string.IsNullOrEmpty(tipoPapelProfissional))
            {
                query.Append(" AND CP.TP_PAPEL = :TipoPapelProfissional ");
                parameters.Add(new OracleParameter("TipoPapelProfissional", tipoPapelProfissional));
            }

            query.Append(" ORDER BY P.NOME");

            return Context.ExecuteStoreQuery<CarteiraProfissionalDTO>(query.ToString(), parameters.ToArray()).ToList();
        }

        public ProfissionalDTO ObterProfissionalSubstitutoPeloID(int profissionalID)
        {
            throw new NotImplementedException();
        }

        public bool SalvarCarteiraProfissional(CarteiraProfissionalDTO carteira)
        {
            StringBuilder query = new StringBuilder();

            query.Append(@"INSERT INTO amesp.gcp_carteira_profissional
                            (ID_TIPOCARTEIRA, ID_PROFISSIONAL, ID_PROFISSIONAL_SUBSTITUTO, TP_PAPEL, VL_LIMITE, ID_EQUIPE, FL_RECEBE_APRAZ) VALUES
                            (:IdTipoCarteira, :IdProfissional, :IdProfissionalSubstituto, :TpPapel, :VlLimite, :IdEquipe, :FlRecebeApraz ) ");

            List<OracleParameter> param = new List<OracleParameter>();
            param.Add(new OracleParameter("IdTipoCarteira", carteira.TipoCarteiraID));
            param.Add(new OracleParameter("IdProfissional", carteira.ProfissionalID));
            param.Add(new OracleParameter("IdProfissionalSubstituto", carteira.ProfissionalSubstitutoID));
            param.Add(new OracleParameter("TpPapel", carteira.TipoPapel));
            param.Add(new OracleParameter("VlLimite", carteira.PontuacaoProfissionalLimite));
            param.Add(new OracleParameter("IdEquipe", carteira.EquipeSaudeID));
            param.Add(new OracleParameter("FlRecebeApraz", carteira.PermiteAprazamento));

            return Convert.ToBoolean(Context.ExecuteStoreCommand(query.ToString(), param.ToArray()));
        }

        public bool EditarCarteiraProfissional(CarteiraProfissionalDTO carteira)
        {
            if (carteira.ProfissionalIDNovo <= 0)
                carteira.ProfissionalIDNovo = carteira.ProfissionalID;

            var query = new StringBuilder();
            var param = new List<OracleParameter>();

            query.Append(@"UPDATE AMESP.GCP_CARTEIRA_PROFISSIONAL 
                           SET 
                                VL_LIMITE = :VlLimite, 
                                ID_PROFISSIONAL_SUBSTITUTO = :IdProfissionalSubstituto, 
                                ID_PROFISSIONAL = :IdProfissionalNovo,
                                FL_RECEBE_APRAZ = :FlRecebeApraz
                           WHERE 
                                ID_PROFISSIONAL = :IdProfissional AND
                                ID_TIPOCARTEIRA = :IdTipoCarteira ");

            param.Add(new OracleParameter(":IdProfissional", carteira.ProfissionalID));
            param.Add(new OracleParameter(":IdProfissionalNovo", carteira.ProfissionalIDNovo));
            param.Add(new OracleParameter(":IdTipoCarteira", carteira.TipoCarteiraID));
            param.Add(new OracleParameter(":VlLimite", carteira.PontuacaoProfissionalLimite));
            param.Add(new OracleParameter(":IdProfissionalSubstituto", carteira.ProfissionalSubstitutoID));
            param.Add(new OracleParameter(":FlRecebeApraz", carteira.PermiteAprazamento));

            if (carteira.EquipeSaudeID.HasValue && carteira.EquipeSaudeID.Value > 0)
            {
                query.Append(" AND ID_EQUIPE = :IdEquipeSaude ");
                param.Add(new OracleParameter(":IdEquipeSaude", carteira.EquipeSaudeID.Value));
            }

            return Convert.ToBoolean(Context.ExecuteStoreCommand(query.ToString(), param.ToArray()));
        }

        public CarteiraProfissionalDTO ObterCarteiraProfissionalPorProfissionalID(long tipoCarteiraID, int IdProfissional, int? equipeID = null)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@"WITH g1 AS (SELECT CAR.ID_CARTEIRA_PROFISSIONAL AS CARTEIRAPROFISSIONALID,
                                              CAR.ID_TIPOCARTEIRA AS TIPOCARTEIRAID,
                                              CAR.ID_PROFISSIONAL AS PROFISSIONALID,
                                              CAR.ID_PROFISSIONAL_SUBSTITUTO AS PROFISSIONALSUBSTITUTOID,
                                              CAR.TP_PAPEL AS TIPOPAPEL,
                                              CAR.ID_EQUIPE AS EQUIPESAUDEID,
                                              CAR.FL_RECEBE_APRAZ AS PERMITEAPRAZAMENTO,
                                              CAR.VL_LIMITE AS PONTUACAOPROFISSIONALLIMITE,
                                              P.NOME AS PROFISSIONALNOME, 
                                              P.NUM_CONSELHO AS PROFISSIONALNUMEROCONSELHO   
                                          FROM 
                                              AMESP.GCP_CARTEIRA_PROFISSIONAL CAR
                                              JOIN AMESP.PROFISSIONAL_SAUDE P ON P.ID_PROFISSIONAL = CAR.ID_PROFISSIONAL
                                          WHERE
                                              CAR.ID_PROFISSIONAL = :IdProfissional
                                              AND CAR.ID_TIPOCARTEIRA = :IdTipoCarteira");

            if (equipeID.HasValue && equipeID.Value > 0)
            {
                query.Append(" AND CAR.ID_EQUIPE = :IdEquipe");
                parameters.Add(new OracleParameter("IdEquipe", equipeID.Value));
            }

            query.Append(@") SELECT (SELECT NOME AS PROFISSIONALNOME
                                     FROM AMESP.PROFISSIONAL_SAUDE 
                                     WHERE ID_PROFISSIONAL = g1.PROFISSIONALSUBSTITUTOID) AS PROFISSIONALSUBSTITUTONOME, 
                                    (SELECT NUM_CONSELHO AS NUMEROCONSELHOSUBSTITUTO 
                                     FROM AMESP.PROFISSIONAL_SAUDE 
                                     WHERE ID_PROFISSIONAL = g1.PROFISSIONALSUBSTITUTOID) AS NUMEROCONSELHOSUBSTITUTO,
                             g1.* from g1");

            parameters.Add(new OracleParameter("IdProfissional", IdProfissional));
            parameters.Add(new OracleParameter("IdTipoCarteira", tipoCarteiraID));

            return Context.ExecuteStoreQuery<CarteiraProfissionalDTO>(query.ToString(), parameters.ToArray()).FirstOrDefault();
        }

        public CarteiraProfissionalDTO ObterCarteiraProfissionalPorConselho(int Conselho)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@"select NOME AS PROFISSIONALNOME, ID_PROFISSIONAL AS ProfissionalID from AMESP.PROFISSIONAL_SAUDE where NUM_CONSELHO = '" + Conselho + "'");

            return Context.ExecuteStoreQuery<CarteiraProfissionalDTO>(query.ToString()).FirstOrDefault();
        }

        public CarteiraProfissionalDTO ObterCarteiraDeEquipePorUnidade(int tipoCarteiraID, int profissionalID, string unidadeID)
        {
            var query = new StringBuilder();
            var parameters = new List<OracleParameter>();

            query.Append(@"SELECT 
                            CAR.ID_CARTEIRA_PROFISSIONAL AS CARTEIRAPROFISSIONALID,
                            CAR.ID_TIPOCARTEIRA AS TIPOCARTEIRAID,
                            CAR.ID_PROFISSIONAL AS PROFISSIONALID,
                            CAR.ID_PROFISSIONAL_SUBSTITUTO AS PROFISSIONALSUBSTITUTOID,
                            CAR.TP_PAPEL AS TIPOPAPEL,
                            CAR.ID_EQUIPE AS EQUIPESAUDEID,
                            CAR.VL_LIMITE AS PONTUACAOPROFISSIONALLIMITE,
                            P.NOME AS PROFISSIONALNOME,
                            (SELECT NOME AS PROFISSIONALNOME
                                    FROM AMESP.PROFISSIONAL_SAUDE 
                                    WHERE ID_PROFISSIONAL = CAR.ID_PROFISSIONAL_SUBSTITUTO
                                    ) AS PROFISSIONALSUBSTITUTONOME
                        FROM 
                          AMESP.GCP_CARTEIRA_PROFISSIONAL CAR
                              JOIN AMESP.PROFISSIONAL_SAUDE P ON P.ID_PROFISSIONAL = CAR.ID_PROFISSIONAL
                              JOIN AMESP.EQUIPE_FAMILIA EF ON EF.ID_EQUIPE = CAR.ID_EQUIPE //GrUPO_FAMILIA
                        WHERE
                          CAR.ID_PROFISSIONAL = :ProfissionalID
                          AND CAR.ID_TIPOCARTEIRA = :TipoCarteiraID
                          AND EF.ID_UNIDADE = :UnidadeID ");

            parameters.Add(new OracleParameter("ProfissionalID", profissionalID));
            parameters.Add(new OracleParameter("TipoCarteiraID", tipoCarteiraID));
            parameters.Add(new OracleParameter("UnidadeID", unidadeID));

            return Context.ExecuteStoreQuery<CarteiraProfissionalDTO>(query.ToString(), parameters.ToArray()).FirstOrDefault();
        }

        public double ObterLimiteAtualProfissional(int IdProfissional, long tipoCarteira, int? idEquipe = null)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@"
                SELECT
                    NVL ((
                        CASE WHEN (TP.TP_LIMITE = 'P') THEN CP.PONTUACAO_ATUAL_CARTEIRA
                        ELSE CP.QTDE_ATUAL_CARTEIRA
                        END
                    ), 0) AS Pontuacao
                FROM AMESP.GCP_CARTEIRA_PROFISSIONAL CP
                  JOIN AMESP.GCP_TIPOCARTEIRA TP ON TP.ID_TIPOCARTEIRA = CP.ID_TIPOCARTEIRA 
                WHERE                     
                    CP.ID_PROFISSIONAL = :IdProfissional AND CP.ID_TIPOCARTEIRA = :TipoCarteiraID ");

            parameters.Add(new OracleParameter("IdProfissional", IdProfissional));
            parameters.Add(new OracleParameter("TipoCarteiraID", tipoCarteira));

            if (idEquipe.HasValue && idEquipe.Value > 0)
            {
                query.Append(" and CP.ID_EQUIPE = :IdEquipe");
                parameters.Add(new OracleParameter("IdEquipe", idEquipe.Value));
            }
            else
            {
                query.Append(" and CP.ID_EQUIPE IS NULL");
            }

            return Context.ExecuteStoreQuery<double>(query.ToString(), parameters.ToArray()).FirstOrDefault();
        }

        public double ObterQuantidadePacientesAtualProfissional(int profissionalID, long tipoCarteiraID, int? equipeID)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@"
                SELECT COUNT (CPE.ID_PESSOA)
                FROM AMESP.GCP_CARTEIRA_PESSOA CPE
                WHERE CPE.ID_PROFISSIONAL = :ProfissionalID AND CPE.ID_TIPOCARTEIRA = :TipoCarteiraID ");

            parameters.Add(new OracleParameter("ProfissionalID", profissionalID));
            parameters.Add(new OracleParameter("TipoCarteiraID", tipoCarteiraID));

            if (equipeID.HasValue)
            {
                query.Append(" AND CPE.ID_EQUIPE = :EquipeID ");
                parameters.Add(new OracleParameter("EquipeID", equipeID.Value));
            }

            return Context.ExecuteStoreQuery<double>(query.ToString(), parameters.ToArray()).FirstOrDefault();
        }

        public List<ListaCarteirasEquipeDTO> ListarCarteirasPorEquipeID(int IdEquipe)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@" select * from amesp.gcp_carteira_profissional cp
                          inner join amesp.glb_profissional_saude ps 
                          on cp.ID_PROFISSIONAL = ps.ID_PROFISSIONAL 
                          inner join amesp.glb_profissional_pessoa pp 
                          on ps.ID_PROFISSIONAL_PESSOA  = pp.ID_PROFISSIONAL_PESSOA ");

            if (!string.IsNullOrEmpty(IdEquipe.ToString()))
            {
                query.Append("where ID_EQUIPE=:id_equipe");
                parameters.Add(new OracleParameter("id_equipe", IdEquipe));
            }

            return Context.ExecuteStoreQuery<ListaCarteirasEquipeDTO>(query.ToString(), parameters.ToArray()).ToList();
        }

        public List<CarteiraProfissionalDTO> ListarCarteirasProfissionaisPorUnidadeID(int tipoCarteiraID, string unidadeID, bool tipoCarteiraIndividual)
        {
            StringBuilder carteiraQuery = new StringBuilder(), unidadeQuery = new StringBuilder();
            string finalQuery = string.Empty;
            var param = new List<OracleParameter>();

            unidadeQuery.Append(@"
                WITH E1 AS (
                  SELECT ID_EQUIPE 
                  FROM AMESP.EQUIPE_FAMILIA E
                  WHERE E.ID_UNIDADE = :UnidadeID
                )
            ");

            carteiraQuery.Append(@"
                SELECT 
                    P.ID_PROFISSIONAL as ProfissionalID, 
                    P.NOME as ProfissionalNome,
                    CP.ID_TIPOCARTEIRA as TipoCarteiraID,
                    CP.TP_PAPEL as TipoPapel,
                    CP.ID_EQUIPE as EquipeSaudeID,
                    TP.FL_ATIVA_LIMITE as LimiteAtivo,
                    TP.TP_LIMITE AS TipoLimite,
                    CP.VL_LIMITE as PontuacaoProfissionalLimite,
                    (
                        CASE WHEN (TP.TP_LIMITE = 'P') THEN CP.PONTUACAO_ATUAL_CARTEIRA
                        ELSE CP.QTDE_ATUAL_CARTEIRA
                        END
                    ) AS PontuacaoProfissionalAtual
                FROM AMESP.GCP_CARTEIRA_PROFISSIONAL CP
                    INNER JOIN AMESP.PROFISSIONAL_SAUDE P ON P.id_profissional = CP.ID_PROFISSIONAL
                    INNER JOIN AMESP.GCP_TIPOCARTEIRA TP ON TP.ID_TIPOCARTEIRA = CP.ID_TIPOCARTEIRA 
                WHERE 
                    CP.ID_TIPOCARTEIRA = :TipoCarteiraID
            ");

            param.Add(new OracleParameter("TipoCarteiraID", tipoCarteiraID));

            if (tipoCarteiraIndividual)
            {
                carteiraQuery.Append(" AND CP.ID_EQUIPE IS NULL ");
                finalQuery = carteiraQuery.ToString();
            }
            else
            {
                carteiraQuery.Append(" AND CP.ID_EQUIPE IN (SELECT ID_EQUIPE FROM E1) ");
                param.Add(new OracleParameter("UnidadeID", unidadeID));
                finalQuery = unidadeQuery.ToString() + carteiraQuery.ToString();
            }

            return Context.ExecuteStoreQuery<CarteiraProfissionalDTO>(finalQuery, param.ToArray()).ToList();
        }

        public List<CarteiraProfissionalDTO> ListarCarteirasPorTipos(int tipoCarteiraID, string papel)
        {
            StringBuilder query = new StringBuilder();
            query.Append(string.Format(@"SELECT P.ID_PROFISSIONAL as ProfissionalID, 
                                                P.NOME as ProfissionalNome,
                                                P.ID_SITUACAO as SituacaoID,
                                                P.ID_CONSELHO as ConselhoID,
                                                C.SIGLA as Sigla,
                                                P.NUM_CONSELHO as ProfissionalNumeroConselho,
                                                CP.ID_PROFISSIONAL_SUBSTITUTO as ProfissionalSubstitutoID,
                                                (CASE WHEN CP.ID_PROFISSIONAL_SUBSTITUTO IS NOT NULL THEN PS.NOME ELSE '' END) AS ProfissionalSubstitutoNome, 
                                                (CASE WHEN CP.ID_PROFISSIONAL_SUBSTITUTO IS NOT NULL THEN PS.NUM_CONSELHO ELSE '' END) AS NumeroConselhoSubstituto,
                                                CP.ID_TIPOCARTEIRA as TipoCarteiraID,
                                                CP.TP_PAPEL as TipoPapel,
                                                CP.VL_LIMITE as PontuacaoProfissionalLimite,
                                                CP.ID_EQUIPE as EquipeSaudeID,
                                                CP.FL_RECEBE_APRAZ as PermiteAprazamento,
                                                TP.FL_ATIVA_LIMITE as LimiteAtivo,
                                                TP.TP_LIMITE AS TipoLimite
                                            FROM AMESP.GCP_CARTEIRA_PROFISSIONAL CP
                                                JOIN AMESP.PROFISSIONAL_SAUDE P ON P.id_profissional = CP.ID_PROFISSIONAL
                                                LEFT JOIN AMESP.PROFISSIONAL_SAUDE PS ON PS.id_profissional = CP.ID_PROFISSIONAL_SUBSTITUTO
                                                JOIN AMESP.GCP_TIPOCARTEIRA TP ON TP.ID_TIPOCARTEIRA = CP.ID_TIPOCARTEIRA 
                                                JOIN AMESP.CONSELHO_PROFISSIONAL C ON P.ID_CONSELHO = C.ID_CONSELHO
                                            WHERE CP.ID_TIPOCARTEIRA=:tipo_CarteiraID AND CP.TP_PAPEL='{0}' 
                                            ORDER BY ProfissionalNome", papel));

            var param = new List<OracleParameter>();
            param.Add(new OracleParameter("tipo_CarteiraID", tipoCarteiraID));

            return Context.ExecuteStoreQuery<CarteiraProfissionalDTO>(query.ToString(), param.ToArray()).ToList();
        }

        public CarteiraPacienteDTO VerificaPacientes(int IdProfissional, int IdCarteira, int? IdEquipe)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> param = new List<OracleParameter>();

            query.Append(@"SELECT CP.ID_PESSOA AS PessoaID FROM AMESP.GCP_CARTEIRA_PESSOA CP ");
            query.Append(@"INNER JOIN amesp.glb_pessoa_operadora PO ON PO.id_pessoa = CP.id_pessoa ");
            query.Append(" WHERE CP.ID_PROFISSIONAL = :IdProfissional ");
            query.Append(" AND CP.ID_TIPOCARTEIRA = :IdCarteira ");
            query.Append(" AND PO.fl_rn <> 1 ");

            param.Add(new OracleParameter("IdProfissional", IdProfissional));
            param.Add(new OracleParameter("IdCarteira", IdCarteira));

            if (IdEquipe.HasValue && IdEquipe.Value > 0)
            {
                query.Append(" AND CP.ID_EQUIPE = :IdEquipe ");
                param.Add(new OracleParameter("IdEquipe", IdEquipe.Value));
            }

            return Context.ExecuteStoreQuery<CarteiraPacienteDTO>(query.ToString(), param.ToArray()).FirstOrDefault();
        }

        public decimal VerificaQuantidadePacientes(int IdTipoCarteira, int IdProfissional, string tipoLimite, int? IdEquipe = null)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> param = new List<OracleParameter>();

            param.Add(new OracleParameter("ID_TIPO_CARTEIRA", IdTipoCarteira));
            param.Add(new OracleParameter("ID_PROFISSIONAL", IdProfissional));


            if (tipoLimite == "Q")
                {
                    query.Append(@"Select count(*) from amesp.gcp_carteira_pessoa md where md.id_profissional = :ID_PROFISSIONAL and md.id_tipocarteira = :ID_TIPO_CARTEIRA");
                }
                else //tipoLimite == "P"
                {
                    query.Append(@"select
                                  sum(
                                      case md.fl_standby when 1 then 0
                                                         when 4 then 0
                                                         else ( 
                                                                case when (md.id_pessoa is not null) then 
                                                                (select ccp.vl_faixa 
                                                                 from amesp.glb_pessoa_fisica pf 
                                                                 inner join amesp.config_carteira_pont ccp on (ccp.fl_genero = pf.id_sexo and amesp.calc_idade(pf.dt_nascimento, sysdate) between ccp.vl_idade_ini and ccp.vl_idade_fim) 
                                                                 where pf.id_pessoa = md.id_pessoa
                                                                 ) else 0 END 
                                                               ) end 
                                      )  LimiteQuantidadeTotal 
                                from amesp.gcp_carteira_pessoa md 
                                where md.id_profissional = :ID_PROFISSIONAL 
                                and md.id_tipocarteira = :ID_TIPO_CARTEIRA");
                }

            if (IdEquipe.HasValue && IdEquipe.Value > 0)
            { 
                param.Add(new OracleParameter("ID_EQUIPE", IdEquipe.Value));
                query.Append(" and md.id_equipe = :ID_EQUIPE");
            }
            return Context.ExecuteStoreQuery<decimal>(query.ToString(), param.ToArray()).FirstOrDefault();
        }

        public bool ExcluiProfissional(int idProfissional, int idCarteira, int? IdEquipe)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"DELETE AMESP.GCP_CARTEIRA_PROFISSIONAL ");
            query.Append(" WHERE ID_PROFISSIONAL = :id_Profissional ");
            query.Append(" AND ID_TIPOCARTEIRA = :idCarteira ");

            List<OracleParameter> param = new List<OracleParameter>();
            param.Add(new OracleParameter("id_Profissional", idProfissional));
            param.Add(new OracleParameter("idCarteira", idCarteira));

            if (IdEquipe.HasValue && IdEquipe.Value > 0)
            {
                query.Append(" AND ID_EQUIPE = :IdEquipe ");
                param.Add(new OracleParameter("IdEquipe", IdEquipe.Value));
            }

            return Convert.ToBoolean(Context.ExecuteStoreCommand(query.ToString(), param.ToArray()));
        }

        public TipoCarteiraLimiteTotalCarteira ObterLimiteTotal(Int64 IdTipoCarteira, string TipoProfissionais, int idProfissional)
        {
            String _querySQL = string.Empty;
            List<OracleParameter> parameters = null;

            try
            {
                parameters = new List<OracleParameter>();
                _querySQL = string.Format(@"
                        select 
                          md.id_profissional IdProfissional,
                          sum(case 
			                    when (md.id_pessoa is not null) 
			                            then (select ccp.vl_faixa from amesp.glb_pessoa_fisica pf 
					                            inner join amesp.config_carteira_pont ccp on (ccp.fl_genero = pf.id_sexo and amesp.calc_idade(pf.dt_nascimento, sysdate) between ccp.vl_idade_ini and ccp.vl_idade_fim) where pf.id_pessoa = md.id_pessoa)
			                    else 0 END) LimiteQuantidadeTotal,
                            (select vl_limite from amesp.gcp_carteira_profissional where id_profissional = :ID_PROFISSIONAL and id_tipocarteira = :ID_TIPO_CARTEIRA)  as LimiteProfissional
	                        from amesp.gcp_carteira_pessoa md 
		                        where md.id_tipocarteira =:ID_TIPO_CARTEIRA
		                          AND md.tp_papel IN('{0}')
                        GROUP BY md.id_profissional", TipoProfissionais);

                parameters.Add(new OracleParameter("ID_TIPO_CARTEIRA", IdTipoCarteira));
                parameters.Add(new OracleParameter("ID_PROFISSIONAL", idProfissional));
                return Context.ExecuteStoreQuery<TipoCarteiraLimiteTotalCarteira>(_querySQL, parameters.ToArray()).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "ObterLimiteTotalCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }

        }

        public TipoCarteiraLimiteTotalCarteira ObterQuantidadeTotal(Int64 IdTipoCarteira, string TipoProfissionais, int idProfissional)
        {
            String _querySQL = string.Empty;
            List<OracleParameter> parameters = null;

            try
            {
                parameters = new List<OracleParameter>();
                _querySQL = string.Format(@"
                         select 
                          md.id_profissional IdProfissional,
                        count(ID_PESSOA) LimiteQuantidadeTotal,
                        (select vl_limite from amesp.gcp_carteira_profissional where id_profissional = :ID_PROFISSIONAL and id_tipocarteira = :ID_TIPO_CARTEIRA)  as LimiteProfissional
	                        from amesp.gcp_carteira_pessoa md 
                         where md.id_tipocarteira =:ID_TIPO_CARTEIRA
		                          AND md.tp_papel IN('{0}')
                        GROUP BY md.id_profissional", TipoProfissionais);

                parameters.Add(new OracleParameter("ID_TIPO_CARTEIRA", IdTipoCarteira));
                parameters.Add(new OracleParameter("ID_PROFISSIONAL", idProfissional));

                return Context.ExecuteStoreQuery<TipoCarteiraLimiteTotalCarteira>(_querySQL, parameters.ToArray()).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "ObterQuantidadeTotalCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }

        }

        /// <summary>
        /// Obtem o limite da carteira
        /// </summary>
        /// <param name="idTipoCarteira">Id do TipoCarteira</param>
        /// <param name="tipoLimite">P - Pontos ou Q - Quantidade</param>
        /// <returns>Limite da carteira</returns>
        public decimal ObterLimiteTipoCarteira(int idTipoCarteira, string tipoLimite, string tipoProfissionais, int idProfissional)
        {
            TipoCarteiraLimiteTotalCarteira ListaProfissionaisLimite = null;

            try
            {
                if (tipoLimite == "P")
                {
                    ListaProfissionaisLimite = ObterLimiteTotal(idTipoCarteira, tipoProfissionais, idProfissional);
                }
                if (tipoLimite == "Q")
                {
                    ListaProfissionaisLimite = ObterQuantidadeTotal(idTipoCarteira, tipoProfissionais, idProfissional);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "ObterLimiteTipoCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }

            return ListaProfissionaisLimite.LimiteQuantidadeTotal;

        }

        public UnidadeEquipe PesquisarUnidadeEquipeCarteiraProfissional(int ProfissionalID, int TipoCarteiraID, string CentroMedico)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@"
                        SELECT DISTINCT           
                               CP.ID_EQUIPE AS IdEquipe,
                               EF.NM_EQUIPE as NomeEquipe,
                               EF.ID_UNIDADE as IdUnidade,
                               U.NM_CENTRO_MEDICO as NomeCentroMedico      
                          FROM amesp.gcp_carteira_profissional cp
                         INNER JOIN amesp.gcp_tipocarteira tc ON tc.id_tipocarteira = cp.id_tipocarteira      
                         INNER JOIN amesp.equipe_familia EF ON EF.ID_EQUIPE = CP.ID_EQUIPE
                         INNER JOIN amesp.cm_unidade U ON U.CENTRO_MEDICO = EF.ID_UNIDADE     
                         WHERE cp.id_profissional = :ID_PROFISSIONAL
                           AND tc.id_tipocarteira = :ID_TIPOCARTEIRA
                           AND ef.id_unidade = :ID_UNIDADE");

            parameters.Add(new OracleParameter("ID_PROFISSIONAL", ProfissionalID));
            parameters.Add(new OracleParameter("ID_TIPOCARTEIRA", TipoCarteiraID));
            parameters.Add(new OracleParameter("ID_UNIDADE", CentroMedico));

            return Context.ExecuteStoreQuery<UnidadeEquipe>(query.ToString(), parameters.ToArray()).FirstOrDefault();
        }

        public List<int> ObterTipoCarteiraPorEquipe(int IdEquipe)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@" select distinct id_tipocarteira 
                                from amesp.gcp_carteira_profissional 
                            where id_equipe =:IdEquipe ");

            parameters.Add(new OracleParameter("IdEquipe", IdEquipe));

            return Context.ExecuteStoreQuery<int>(query.ToString(), parameters.ToArray()).ToList<int>();
        }

        public int AtualizarPontuacaoAtualCarteirasPorProfissional(int tipoCarteira, List<int> ProfissionaisIDs)
        {
            int totalUpdates = 0;
            string profissionais = string.Join(",", ProfissionaisIDs.ToArray());
            string procedure = "AMESP.PROC_CALC_PONT_PROFISSIONAL";

            //PARAMETROS
            var paramList = new List<OracleParameter>();
            paramList.Add(new OracleParameter
            {
                ParameterName = "p_tipocarteira",
                OracleDbType = OracleDbType.Integer,
                Direction = System.Data.ParameterDirection.Input,
                Value = tipoCarteira
            });
            paramList.Add(new OracleParameter
            {
                ParameterName = "p_listaprofissional",
                OracleDbType = OracleDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Value = profissionais
            });
            paramList.Add(new OracleParameter
            {
                ParameterName = "QTDE_ATUALIZADOS",
                OracleDbType = OracleDbType.Integer,
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = 0
            });

            try
            {
                //EXECUTANDO PROCEDURE
                var retorno = Context.ExecuteStoreProcedure(procedure, paramList.ToArray());

                int.TryParse(retorno["QTDE_ATUALIZADOS"].Value.ToString(), out totalUpdates);
                return totalUpdates;
            }
            catch
            {
                return 0;
            }
            
        }

        public int AtualizarPontuacaoAtualCarteirasPorTipoCarteira(int tipoCarteira)
        {
            int totalUpdates = 0;
            string procedure = "AMESP.PROC_CALC_PONT_TIPOCARTEIRA";

            //PARAMETROS
            var paramList = new List<OracleParameter>();
            paramList.Add(new OracleParameter
            {
                ParameterName = "p_tipocarteira",
                OracleDbType = OracleDbType.Integer,
                Direction = System.Data.ParameterDirection.Input,
                Value = tipoCarteira
            });
            paramList.Add(new OracleParameter
            {
                ParameterName = "QTDE_ATUALIZADOS",
                OracleDbType = OracleDbType.Integer,
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = 0
            });

            try
            {
                //EXECUTANDO PROCEDURE
                var retorno = Context.ExecuteStoreProcedure(procedure, paramList.ToArray());

                int.TryParse(retorno["QTDE_ATUALIZADOS"].Value.ToString(), out totalUpdates);
                return totalUpdates;
            }
            catch
            {
                return 0;
            }
        }

        public int AtualizarPontuacaoCarteiraProfissional(int tipoCarteiraID, int profissionalID, int? equipeID)
        {
            var command = new StringBuilder();
            var parameters = new List<OracleParameter>();

            command.Append(@" DECLARE totalBenef NUMBER := 0; totalPontosBenef NUMBER := 0; ");
            command.Append(" BEGIN ");

            command.Append(@"
                SELECT COUNT(*), NVL(SUM(pontuacao), 0)
                INTO totalBenef, totalPontosBenef
                FROM (
                    SELECT cp.ID_PESSOA, (
                        SELECT F.VL_FAIXA FROM AMESP.CONFIG_CARTEIRA_PONT F
                        WHERE F.FL_GENERO = PF.ID_SEXO AND ( AMESP.CALC_IDADE(PF.DT_NASCIMENTO) BETWEEN F.VL_IDADE_INI AND F.VL_IDADE_FIM )
                    ) AS pontuacao
                    FROM amesp.GCP_CARTEIRA_PESSOA cp
                        INNER JOIN AMESP.GLB_PESSOA_FISICA pf ON pf.ID_PESSOA = cp.ID_PESSOA
                    WHERE
                        cp.ID_TIPOCARTEIRA = :tipoCarteiraID
                        AND cp.ID_PROFISSIONAL = :profissionalID
                        AND cp.FL_STANDBY = 0
                        {{EQUIPE_CLAUSE}} 
                ); "
            );

            command.Append(@"
                UPDATE AMESP.GCP_CARTEIRA_PROFISSIONAL cp
                SET QTDE_ATUAL_CARTEIRA = totalBenef, PONTUACAO_ATUAL_CARTEIRA = totalPontosBenef
                WHERE
                    cp.ID_TIPOCARTEIRA = :tipoCarteiraID
                    AND cp.ID_PROFISSIONAL = :profissionalID
                    {{EQUIPE_CLAUSE}}; "
            );

            command.Append(" END; ");

            parameters.Add(new OracleParameter("tipoCarteiraID", tipoCarteiraID));
            parameters.Add(new OracleParameter("profissionalID", profissionalID));

            if (equipeID.HasValue && equipeID.Value > 0)
            {
                command.Replace("{{EQUIPE_CLAUSE}}", " AND cp.ID_EQUIPE = :equipeID ");
                parameters.Add(new OracleParameter("equipeID", equipeID.Value));
            }
            else
            {
                command.Replace("{{EQUIPE_CLAUSE}}", " AND cp.ID_EQUIPE IS NULL ");
            }

            return Context.ExecuteStoreCommand(command.ToString(), parameters.ToArray());
        }
    }
}

