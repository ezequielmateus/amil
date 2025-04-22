using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Amil.SisMedWebCad.DAL.wsSisMedWeb;
using DTO;
using Amil.Framework.Security.Web;
using Oracle.DataAccess.Client;
using DAL;
using System.Runtime.Remoting.Contexts;
using System.Data;

namespace Amil.SisMedWebCad.DAL
{
    public class dbCarteiraProfissional
    {
        /// <summary>
        /// Salva Carteira
        /// </summary>
        /// <param name="Carteira"></param>
        /// <returns>ID da carteira</returns>
        public bool SalvarCarteiraProfissional(tpCarteiraProfissional carteira)
        {
            try
            {
                using (var client = dbUtil.GetWCFInstance())
                    return client.SalvarCarteiraProfissional(TransformToWSFromDTO(carteira));
            }
            catch (FaultException<AmilBussinessFault> ex)
            {
                throw new BusinessException(ex);
            }
        }

        /// <summary>
        /// Editar Carteira
        /// </summary>
        /// <param name="Carteira"></param>
        /// <returns>ID da carteira</returns>
        public bool EditarCarteiraProfissional(tpCarteiraProfissional carteira)
        {
            try
            {
                using (var client = dbUtil.GetWCFInstance())
                    return client.EditarCarteiraProfissional(TransformToWSFromDTO(carteira));
            }
            catch (FaultException<AmilBussinessFault> ex)
            {
                throw new BusinessException(ex);
            }
        }

        /// <summary>
        /// Exclui o Profissional
        /// </summary>
        /// <param name="idProfissional">Id do Profissional</param>
        public void Excluir(int idProfissional, int idCarteira, int? idEquipe)
        {
            try
            {
                using (var client = dbUtil.GetWCFInstance())
                    client.ExcluiProfissional(idProfissional, idCarteira, idEquipe);
            }
            catch (FaultException<AmilBussinessFault> ex)
            {
                throw new BusinessException(ex);
            }
        }

        /// <summary>
        /// Lista carteiras
        /// </summary>
        /// <param name="tipoCarteiraID"></param>
        /// <param name="papel"></param>
        /// <returns>Lista de carteiras</returns>
        public List<tpCarteiraProfissional> ListarCarteirasPorTipos(int tipoCarteiraID, string papel)
        {
            try
            {
                using (var client = dbUtil.GetWCFInstance())
                    return TransformToDTOFromWS(client.ListarCarteirasPorTipos(tipoCarteiraID, papel));
            }
            catch (FaultException<AmilBussinessFault> ex)
            {
                throw new BusinessException(ex);
            }
        }

        /// <summary>
        /// Lista carteiras
        /// </summary>
        /// <param name="tipoCarteiraID"></param>
        /// <param name="papel"></param>
        /// <returns>Lista de carteiras</returns>
        public tpCarteiraProfissional BuscarCarteiraPorIDProfissional(int tipoCarteiraID, int profissionalID, int? equipeSaudeID)
        {
            try
            {
                using (var client = dbUtil.GetWCFInstance())
                    return TransformToDTOFromWS(client.ObterCarteiraProfissionalPorProfissionalID(tipoCarteiraID, profissionalID, equipeSaudeID));
            }
            catch (FaultException<AmilBussinessFault> ex)
            {
                throw new BusinessException(ex);
            }
        }

        public tpCarteiraProfissional BuscarCarteiraDeEquipePorUnidadeID(int tipoCarteiraID, int profissionalID, string unidadeID)
        {
            using (var client = dbUtil.GetWCFInstance())
                return TransformToDTOFromWS(client.ObterCarteiraDeEquipePorUnidade(tipoCarteiraID, profissionalID, unidadeID));
        }

        /// <summary>
        /// Lista carteiras
        /// </summary>
        /// <param name="tipoCarteiraID"></param>
        /// <param name="papel"></param>
        /// <returns>Lista de carteiras</returns>
        public tpCarteiraProfissional BuscarCarteiraPorConselho(int Conselho)
        {
            try
            {
                using (var client = dbUtil.GetWCFInstance())
                    return TransformToDTOFromWS(client.ObterCarteiraProfissionalPorConselho(Conselho));
            }
            catch (FaultException<AmilBussinessFault> ex)
            {
                throw new BusinessException(ex);
            }
        }

        /// <summary>
        /// Buscar carteiras
        /// </summary>
        /// <param name="tipoCarteiraID"></param>
        /// <param name="equipeSaudeID"></param>
        /// <returns>Lista de carteiras</returns>
        public List<tpCarteiraProfissional> BuscarCarteira(int tipoCarteiraID, int equipeSaudeID)
        {
            try
            {
                using (var client = dbUtil.GetWCFInstance())
                    return TransformToDTOFromWS(client.ListarProfissionaisPorTipoCarteiraOuEquipeSaude(tipoCarteiraID, equipeSaudeID));
            }
            catch (FaultException<AmilBussinessFault> ex)
            {
                throw new BusinessException(ex);
            }
        }

        private tpTipoEquipeSaudeProfissional ConvertTipoProfissional(string tipo)
        {
            switch (tipo)
            {
                case "M":
                    return tpTipoEquipeSaudeProfissional.Medico;
                case "E":
                    return tpTipoEquipeSaudeProfissional.Enfermeiro;
                case "A":
                    return tpTipoEquipeSaudeProfissional.AgenteSaude;
                default:
                    return tpTipoEquipeSaudeProfissional.Medico;
            }
        }

        private List<tpCarteiraProfissional> TransformToDTOFromWS(List<CarteiraProfissionalDTO> list)
        {
            return list.Select(p => TransformToDTOFromWS(p)).ToList();
        }

        private tpCarteiraProfissional TransformToDTOFromWS(CarteiraProfissionalDTO carteira)
        {
            if (carteira == null)
                return null;

            return new tpCarteiraProfissional
            {
                ProfissionalID = carteira.ProfissionalID,
                ProfissionalIDNovo = carteira.ProfissionalIDNovo,
                Nome = carteira.ProfissionalNome,
                NumeroConselho = carteira.ProfissionalNumeroConselho,
                LimiteTotalCarteira = carteira.PontuacaoProfissionalLimite,
                TipoProfissional = ConvertTipoProfissional(carteira.TipoPapel),
                SiglaConselho = carteira.Sigla,
                ProfissionalSubstitutoID = carteira.ProfissionalSubstitutoID,
                NomeSubstituto = carteira.ProfissionalSubstitutoNome,
                NumeroConselhoSubstituto = carteira.NumeroConselhoSubstituto,
                PermiteAprazamento = carteira.PermiteAprazamento
            };
        }

        private CarteiraProfissionalDTO TransformToWSFromDTO(tpCarteiraProfissional carteira)
        {
            if (carteira == null)
                return null;

            return new CarteiraProfissionalDTO
            {
                TipoCarteiraID = carteira.TipoCarteiraID,
                EquipeSaudeID = carteira.EquipeSaudeID,
                ProfissionalNome = carteira.Nome,
                ProfissionalID = carteira.ProfissionalID,
                ProfissionalIDNovo = carteira.ProfissionalIDNovo,
                ProfissionalSubstitutoID = carteira.ProfissionalSubstitutoID,
                ProfissionalSubstitutoNome = carteira.NomeSubstituto,
                PontuacaoProfissionalLimite = carteira.LimiteTotalCarteira,
                TipoPapel = carteira.Papel,
                PermiteAprazamento = carteira.PermiteAprazamento
            };
        }


        public tpCarteiraProfissional BuscarCarteiraDeGrupoPorUnidadeID(int tipoCarteiraID, int profissionalID, string unidadeID)
        {
            try
            {
                var query = new StringBuilder();
                var parameters = new List<OracleParameter>();

                query.Append(@"SELECT 
                            CAR.ID_CARTEIRA_PROFISSIONAL AS CARTEIRAPROFISSIONALID,
                            CAR.ID_TIPOCARTEIRA AS TIPOCARTEIRAID,
                            CAR.ID_PROFISSIONAL AS PROFISSIONALID,
                            CAR.ID_PROFISSIONAL_SUBSTITUTO AS PROFISSIONALSUBSTITUTOID,
                            CAR.TP_PAPEL AS TIPOPAPEL,
                            CAR.ID_GRUPO AS GRUPOFAMILIAID,
                            CAR.VL_LIMITE AS PONTUACAOPROFISSIONALLIMITE,
                            P.NOME AS PROFISSIONALNOME,
                            (SELECT NOME AS PROFISSIONALNOME
                                    FROM AMESP.PROFISSIONAL_SAUDE 
                                    WHERE ID_PROFISSIONAL = CAR.ID_PROFISSIONAL_SUBSTITUTO
                                    ) AS PROFISSIONALSUBSTITUTONOME
                        FROM 
                          AMESP.GCP_CARTEIRA_PROFISSIONAL CAR
                              JOIN AMESP.PROFISSIONAL_SAUDE P ON P.ID_PROFISSIONAL = CAR.ID_PROFISSIONAL
                              JOIN AMESP.GRUPO_FAMILIA GF ON GF.ID_GRUPO = CAR.ID_GRUPO
                        WHERE
                          CAR.ID_PROFISSIONAL = :ProfissionalID
                          AND CAR.ID_TIPOCARTEIRA = :TipoCarteiraID
                          AND GF.ID_UNIDADE = :UnidadeID ");

                parameters.Add(new OracleParameter("ProfissionalID", profissionalID));
                parameters.Add(new OracleParameter("TipoCarteiraID", tipoCarteiraID));
                parameters.Add(new OracleParameter("UnidadeID", unidadeID));

                var dr = OracleHelper.ExecuteReader(query.ToString(), parameters.ToArray());

                if (dr.HasRows && dr.Read())

                {

                    // Verificar quais colunas estão disponíveis

                    var schema = dr.GetSchemaTable();

                    var columnNames = new List<string>();

                    foreach (DataRow row in schema.Rows)
                    {
                        columnNames.Add(row["ColumnName"].ToString());
                    }                    

                    var carteira = new tpCarteiraProfissional();

                    if (columnNames.Contains("TIPOCARTEIRAID"))
                        carteira.TipoCarteiraID = Convert.ToInt32(dr["TIPOCARTEIRAID"]);

                    if (columnNames.Contains("GRUPOFAMILIAID"))
                        carteira.GrupoFamiliaID = dr["GRUPOFAMILIAID"] != DBNull.Value ? Convert.ToInt32(dr["GRUPOFAMILIAID"]) : (int?)null;

                    if (columnNames.Contains("PROFISSIONALID"))
                        carteira.ProfissionalID = Convert.ToInt32(dr["PROFISSIONALID"]);

                    if (columnNames.Contains("PROFISSIONALNOME"))
                        carteira.Nome = Convert.ToString(dr["PROFISSIONALNOME"]);

                    if (columnNames.Contains("NUMEROCONSELHO"))
                        carteira.NumeroConselho = dr["NUMEROCONSELHO"] != DBNull.Value ? Convert.ToString(dr["NUMEROCONSELHO"]) : null;

                    if (columnNames.Contains("PROFISSIONALSUBSTITUTOID"))
                        carteira.ProfissionalSubstitutoID = dr["PROFISSIONALSUBSTITUTOID"] != DBNull.Value ? Convert.ToInt32(dr["PROFISSIONALSUBSTITUTOID"]) : (int?)null;

                    if (columnNames.Contains("PROFISSIONALSUBSTITUTONOME"))
                        carteira.NomeSubstituto = dr["PROFISSIONALSUBSTITUTONOME"] != DBNull.Value ? Convert.ToString(dr["PROFISSIONALSUBSTITUTONOME"]) : null;

                    if (columnNames.Contains("NUMEROCONSELHOSUBSTITUTO"))
                        carteira.NumeroConselhoSubstituto = dr["NUMEROCONSELHOSUBSTITUTO"] != DBNull.Value ? Convert.ToString(dr["NUMEROCONSELHOSUBSTITUTO"]) : null;

                    if (columnNames.Contains("PONTUACAOPROFISSIONALLIMITE"))
                        carteira.LimiteTotalCarteira = dr["PONTUACAOPROFISSIONALLIMITE"] != DBNull.Value ? Convert.ToDouble(dr["PONTUACAOPROFISSIONALLIMITE"]) : (double?)null;

                    if (columnNames.Contains("TIPOPAPEL"))
                        carteira.Papel = Convert.ToString(dr["TIPOPAPEL"]);

                    if (columnNames.Contains("PERMITEAPRAZAMENTO"))
                        carteira.PermiteAprazamento = dr["PERMITEAPRAZAMENTO"] != DBNull.Value ? Convert.ToInt32(dr["PERMITEAPRAZAMENTO"]) : 0;

                    dr.Close();

                    return carteira;
                }
                dr.Close();
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar carteira de grupo por unidade: " + ex.Message, ex);
            }

        }

        public bool SalvarCarteiraProfissionalGrupo(tpCarteiraProfissional carteira)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.Append(@"INSERT INTO amesp.gcp_carteira_profissional
                (ID_TIPOCARTEIRA, ID_PROFISSIONAL, ID_PROFISSIONAL_SUBSTITUTO, TP_PAPEL, VL_LIMITE, ID_GRUPO, FL_RECEBE_APRAZ) VALUES
                (:IdTipoCarteira, :IdProfissional, :IdProfissionalSubstituto, :TpPapel, :VlLimite, :IdGrupo, :FlRecebeApraz)");
                var parametros = new OracleParameter[]
                {
                    new OracleParameter("IdTipoCarteira", carteira.TipoCarteiraID),
                    new OracleParameter("IdProfissional", carteira.ProfissionalIDNovo),
                    new OracleParameter("IdProfissionalSubstituto", carteira.ProfissionalSubstitutoID ?? (object)DBNull.Value),
                    new OracleParameter("TpPapel", carteira.Papel),
                    new OracleParameter("VlLimite", carteira.LimiteTotalCarteira ?? (object)DBNull.Value),
                    new OracleParameter("IdGrupo", carteira.GrupoFamiliaID),
                    new OracleParameter("FlRecebeApraz", carteira.PermiteAprazamento)
                };

                OracleHelper.ExecuteNonQuery(query.ToString(), parametros);
                return true;
            }

            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar carteira de grupo: " + ex.Message, ex);
            }

        }

        public List<tpCarteiraProfissional> BuscarCarteiraGrupo(int tipoCarteiraID, int? grupoFamiliaID, string tipoPapelProfissional)
        {
            StringBuilder query = new StringBuilder();

            List<OracleParameter> parameters = new List<OracleParameter>();
            query.Append(@"
                    SELECT
                    CAR.ID_CARTEIRA_PROFISSIONAL AS CARTEIRAPROFISSIONALID,
                    P.ID_PROFISSIONAL as PROFISSIONALID,
                    P.NOME as PROFISSIONALNOME,
                    P.ID_SITUACAO as SITUACAOID,
                    P.ID_CONSELHO as CONSELHOID,
                    C.SIGLA as SIGLACONSELHO,
                    P.NUM_CONSELHO as NUMEROCONSELHO,
                    CAR.ID_PROFISSIONAL_SUBSTITUTO as PROFISSIONALSUBSTITUTOID,
                    (CASE WHEN CAR.ID_PROFISSIONAL_SUBSTITUTO IS NOT NULL THEN PS.NOME ELSE '' END) AS NOMESUBSTITUTO,
                    (CASE WHEN CAR.ID_PROFISSIONAL_SUBSTITUTO IS NOT NULL THEN PS.NUM_CONSELHO ELSE '' END) AS NUMEROCONSELHOSUBSTITUTO,
                    CAR.ID_TIPOCARTEIRA as TIPOCARTEIRAID,
                    CAR.TP_PAPEL as PAPEL,
                    CAR.VL_LIMITE as LIMITETOTALCARTEIRA,
                    (CASE WHEN (TP.TP_LIMITE = 'P') THEN (SELECT NVL (SUM(CASE MD.FL_STANDBY WHEN 1 THEN 0
                    WHEN 4 THEN 0
                    ELSE (
                    CASE WHEN (MD.ID_PESSOA IS NOT NULL) THEN
                    (SELECT CCP.VL_FAIXA
                    FROM AMESP.GLB_PESSOA_FISICA PF
                    INNER JOIN AMESP.CONFIG_CARTEIRA_PONT CCP ON (CCP.FL_GENERO = PF.ID_SEXO AND AMESP.CALC_IDADE(PF.DT_NASCIMENTO, SYSDATE) BETWEEN CCP.VL_IDADE_INI AND CCP.VL_IDADE_FIM)
                    WHERE PF.ID_PESSOA = MD.ID_PESSOA
                    ) ELSE 0 END
                    ) END
                    ), 0) LIMITEQUANTIDADETOTAL
                    FROM AMESP.GCP_CARTEIRA_PESSOA MD
                    WHERE MD.ID_PROFISSIONAL = CAR.ID_PROFISSIONAL
                    AND MD.ID_TIPOCARTEIRA = CAR.ID_TIPOCARTEIRA
                    AND MD.ID_GRUPO = CAR.ID_GRUPO
                    AND MD.TP_PAPEL = CAR.TP_PAPEL
                    )
                    ELSE CAR.QTDE_ATUAL_CARTEIRA
                    END
                    ) AS VALORATUALCARTEIRA,
                    CAR.ID_GRUPO as GRUPOFAMILIAID,
                    CAR.FL_RECEBE_APRAZ as PERMITEAPRAZAMENTO,
                    TP.FL_ATIVA_LIMITE as LIMITEPACIENTESATIVO,
                    TP.TP_LIMITE AS TIPOLIMITE
                    FROM AMESP.GCP_CARTEIRA_PROFISSIONAL CAR
                    INNER JOIN AMESP.PROFISSIONAL_SAUDE P ON P.id_profissional = CAR.ID_PROFISSIONAL
                    INNER JOIN AMESP.GCP_TIPOCARTEIRA TP ON TP.ID_TIPOCARTEIRA = CAR.ID_TIPOCARTEIRA
                    INNER JOIN AMESP.CONSELHO_PROFISSIONAL C ON P.ID_CONSELHO = C.ID_CONSELHO
                    LEFT JOIN AMESP.PROFISSIONAL_SAUDE PS ON PS.id_profissional = CAR.ID_PROFISSIONAL_SUBSTITUTO
                    WHERE
                    CAR.ID_TIPOCARTEIRA = :TipoCarteiraID ");

            parameters.Add(new OracleParameter("TipoCarteiraID", tipoCarteiraID));
            if (grupoFamiliaID.HasValue && grupoFamiliaID.Value > 0)
            {
                query.Append(" AND CAR.ID_GRUPO = :GrupoFamiliaID ");
                parameters.Add(new OracleParameter("GrupoFamiliaID", grupoFamiliaID));
            }
            else
            {
                query.Append(" AND CAR.ID_GRUPO IS NULL ");
            }
            if (!string.IsNullOrEmpty(tipoPapelProfissional))
            {
                query.Append(" AND CAR.TP_PAPEL = :TipoPapelProfissional ");
                parameters.Add(new OracleParameter("TipoPapelProfissional", tipoPapelProfissional));
            }
            query.Append(" ORDER BY P.NOME");            

            var dr = OracleHelper.ExecuteReader(query.ToString(), parameters.ToArray());
            List<tpCarteiraProfissional> result = new List<tpCarteiraProfissional>();

            while (dr.Read())
            {
                var schema = dr.GetSchemaTable();
                var columnNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    columnNames.Add(row["ColumnName"].ToString());
                }
                
                tpCarteiraProfissional carteira = new tpCarteiraProfissional();                

                if (columnNames.Contains("CARTEIRAPROFISSIONALID"))
                    carteira.CarteiraProfissionalID = dr["CARTEIRAPROFISSIONALID"] != DBNull.Value ? Convert.ToInt32(dr["CARTEIRAPROFISSIONALID"]) : 0;

                if (columnNames.Contains("TIPOCARTEIRAID"))
                    carteira.TipoCarteiraID = dr["TIPOCARTEIRAID"] != DBNull.Value ? Convert.ToInt32(dr["TIPOCARTEIRAID"]) : 0;

                if (columnNames.Contains("PROFISSIONALID"))
                    carteira.ProfissionalID = dr["PROFISSIONALID"] != DBNull.Value ? Convert.ToInt32(dr["PROFISSIONALID"]) : 0;

                if (columnNames.Contains("PROFISSIONALID")) 
                    carteira.ProfissionalIDNovo = dr["PROFISSIONALID"] != DBNull.Value ? Convert.ToInt32(dr["PROFISSIONALID"]) : 0;

                if (columnNames.Contains("PROFISSIONALSUBSTITUTOID"))
                    carteira.ProfissionalSubstitutoID = dr["PROFISSIONALSUBSTITUTOID"] != DBNull.Value ? Convert.ToInt32(dr["PROFISSIONALSUBSTITUTOID"]) : (int?)null;

                if (columnNames.Contains("GRUPOFAMILIAID"))
                    carteira.GrupoFamiliaID = dr["GRUPOFAMILIAID"] != DBNull.Value ? Convert.ToInt32(dr["GRUPOFAMILIAID"]) : (int?)null;

                if (columnNames.Contains("PERMITEAPRAZAMENTO"))
                    carteira.PermiteAprazamento = dr["PERMITEAPRAZAMENTO"] != DBNull.Value ? Convert.ToInt32(dr["PERMITEAPRAZAMENTO"]) : 0;

                if (columnNames.Contains("SIGLACONSELHO"))
                    carteira.SiglaConselho = dr["SIGLACONSELHO"] != DBNull.Value ? Convert.ToString(dr["SIGLACONSELHO"]) : string.Empty;

                if (columnNames.Contains("PROFISSIONALNOME"))
                    carteira.Nome = dr["PROFISSIONALNOME"] != DBNull.Value ? Convert.ToString(dr["PROFISSIONALNOME"]) : string.Empty;

                if (columnNames.Contains("PAPEL"))
                    carteira.Papel = dr["PAPEL"] != DBNull.Value ? Convert.ToString(dr["PAPEL"]) : string.Empty;

                if (columnNames.Contains("NUMEROCONSELHO"))
                    carteira.NumeroConselho = dr["NUMEROCONSELHO"] != DBNull.Value ? Convert.ToString(dr["NUMEROCONSELHO"]) : string.Empty;

                if (columnNames.Contains("NOMESUBSTITUTO"))
                    carteira.NomeSubstituto = dr["NOMESUBSTITUTO"] != DBNull.Value ? Convert.ToString(dr["NOMESUBSTITUTO"]) : string.Empty;

                if (columnNames.Contains("NUMEROCONSELHOSUBSTITUTO"))
                    carteira.NumeroConselhoSubstituto = dr["NUMEROCONSELHOSUBSTITUTO"] != DBNull.Value ? Convert.ToString(dr["NUMEROCONSELHOSUBSTITUTO"]) : string.Empty;

                if (columnNames.Contains("LIMITEPACIENTESATIVO"))
                    carteira.LimitePacientesAtivo = dr["LIMITEPACIENTESATIVO"] != DBNull.Value ? Convert.ToBoolean(dr["LIMITEPACIENTESATIVO"]) : false;

                if (columnNames.Contains("TIPOLIMITE"))
                    carteira.TipoLimite = dr["TIPOLIMITE"] != DBNull.Value ? Convert.ToString(dr["TIPOLIMITE"]) : string.Empty;

                if (columnNames.Contains("LIMITETOTALCARTEIRA"))
                    carteira.LimiteTotalCarteira = dr["LIMITETOTALCARTEIRA"] != DBNull.Value ? Convert.ToDouble(dr["LIMITETOTALCARTEIRA"]) : (double?)null;

                if (columnNames.Contains("VALORATUALCARTEIRA"))
                    carteira.ValorAtualCarteira = dr["VALORATUALCARTEIRA"] != DBNull.Value ? Convert.ToDouble(dr["VALORATUALCARTEIRA"]) : (double?)null;                

                if (columnNames.Contains("PAPEL"))
                {
                    string papel = dr["PAPEL"] != DBNull.Value ? Convert.ToString(dr["PAPEL"]) : string.Empty;
                    switch (papel)
                    {
                        case "M":
                            carteira.TipoProfissional = tpTipoEquipeSaudeProfissional.Medico;
                            break;
                        case "E":
                            carteira.TipoProfissional = tpTipoEquipeSaudeProfissional.Enfermeiro;
                            break;
                        case "A":
                            carteira.TipoProfissional = tpTipoEquipeSaudeProfissional.AgenteSaude;
                            break;
                        default:                   
                            carteira.TipoProfissional = tpTipoEquipeSaudeProfissional.Medico;
                            break;
                    }
                }

                result.Add(carteira);
            }

            dr.Close();
            return result;
        }
    }
}
