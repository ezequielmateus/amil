using Amil.Framework.Data;
using Amil.SisMed.DAL.Transformer;
using Amil.SisMed.DTO;
using DataModel;
using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;

namespace Amil.SisMed.DAL
{
    public class TipoCarteiraDAO : BaseDAO
    {
        internal string _mensagemErro = "Ocorreu um erro durante a operação : {0} , detalhes do erro : {1}";


        public TipoCarteiraDTO PesquisarTipoCarteiraPorId(string idTipoCarteira)
        {
            String _querySQL = string.Empty;
            List<OracleParameter> parameters = null;
            TipoCarteiraDTO TipoCarteira = null;

            try
            {
                parameters = new List<OracleParameter>();
                _querySQL = @" SELECT
                                t.ID_TIPOCARTEIRA IdTipoCarteira,
                                t.DS_TIPOCARTEIRA Descricao,
                                t.CD_TIPOCARTEIRA Codigo,
                                t.ST_ATIVO Ativo,
                                t.FL_MEDICO FlagMedico,
                                t.FL_ENFERMEIRO FlagEnfermeiro,
                                t.FL_AGENTESAUDE FlagAgenteSaude,
                                t.IND_GRUPO_INDIV IndicadorGrpIndividual,
                                t.FL_ATIVA_LIMITE AtivaLimite,
                                t.TP_LIMITE  TipoLimite,
                                t.FL_HABILITA_LISTA_CONTATOS HabilitaListaContato,
                                t.QT_MAX_CONTATOS QtMaximaContato,
                                t.QT_TENTATIVAS_DIA QtTentativasDia,
                                t.QT_DIAS_REAGENDAR QtDiasReagendar,
                                t.QT_DIAS_RETORNO QtDiasRetorno,
                                t.FL_SINALIZA_PACIENTE FlagSinalizaPaciente,
                                t.FL_PERMITE_ASSOCIACAO FlagPermiteAssociacao,
                                t.FL_PERMITE_CAPTACAO FlagPermiteCaptacao,
                                t.FL_ESPECIALIDADE FlagEspecialidade,
                                t.FL_PERMITE_EXCLUSAO_CARTEIRA FlagPermissaoExclusao,
                                t.DT_CRIACAO DataCriacao,
                                t.USR_CRIACAO UsuarioCriacao,
                                t.DT_ULT_ALTERACAO DataUltimaAlteracao,
                                t.USR_ULT_ALTERACAO UsuarioUltAlteracao,
                                t.FL_PERMITE_EDICAO_PROGRAMA
                             FROM AMESP.GCP_TIPOCARTEIRA T
                                WHERE t.ID_TIPOCARTEIRA = :ID_TIPO_CARTEIRA";

                parameters.Add(new OracleParameter(":ID_TIPO_CARTEIRA", idTipoCarteira));

                if (idTipoCarteira != "")
                    TipoCarteira = Context.ExecuteStoreQuery<TipoCarteiraDTO>(_querySQL, parameters.ToArray()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "PesquisarTipoCarteiraPorId", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }
            return TipoCarteira;

        }


        /// <summary>
        /// Pesquisa os tipos de carteira cadastradas;
        /// </summary>
        /// <param name="tipoCarteiraDescricao">Descricao do tipo de carteira</param>
        /// <param name="obterTodos">Obter todos os registros</param>
        /// <returns>Lista de tipo de carteira</returns>
        public List<TipoCarteiraDTO> PesquisarTiposCarteira(string tipoCarteiraDescricao, bool obterTodos, bool apenasAtivos)
        {
            string query_ = string.Empty;
            List<OracleParameter> parameters = null;
            List<TipoCarteiraDTO> ListaTipoCarteira = null;

            try
            {
                parameters = new List<OracleParameter>();

                query_ = (@" SELECT  
                                t.ID_TIPOCARTEIRA IdTipoCarteira,
                                t.DS_TIPOCARTEIRA Descricao,
                                t.CD_TIPOCARTEIRA Codigo,
                                t.ST_ATIVO Ativo,
                                t.FL_MEDICO FlagMedico,
                                t.FL_ENFERMEIRO FlagEnfermeiro,
                                t.FL_AGENTESAUDE FlagAgenteSaude,
                                t.IND_GRUPO_INDIV IndicadorGrpIndividual,
                                t.FL_ATIVA_LIMITE AtivaLimite,
                                t.TP_LIMITE TipoLimite,
                                t.FL_HABILITA_LISTA_CONTATOS HabilitaListaContato,
                                t.QT_MAX_CONTATOS QtMaximaContato,
                                t.QT_TENTATIVAS_DIA QtTentativasDia,
                                t.QT_DIAS_REAGENDAR QtDiasReagendar,
                                t.QT_DIAS_RETORNO QtDiasRetorno,
                                t.FL_SINALIZA_PACIENTE FlagSinalizaPaciente,
                                t.FL_PERMITE_ASSOCIACAO FlagPermiteAssociacao,
                                t.FL_PERMITE_CAPTACAO FlagPermiteCaptacao,
                                t.FL_STANDBY FlagStandBy,
                                t.FL_ESPECIALIDADE FlagEspecialidade,
                                t.FL_PERMITE_EXCLUSAO_CARTEIRA FlagPermissaoExclusao,
                                t.DT_CRIACAO DataCriacao,
                                t.USR_CRIACAO UsuarioCriacao,
                                t.DT_ULT_ALTERACAO DataUltimaAlteracao,
                                t.USR_ULT_ALTERACAO UsuarioUltAlteracao,
                                t.FL_PERMITE_EDICAO_PROGRAMA
                               FROM AMESP.GCP_TIPOCARTEIRA t 
                               WHERE 0 = 0 ");

                if (apenasAtivos)
                    query_ += " AND t.ST_ATIVO = 1 ";

                if (!obterTodos)
                {
                    query_ += " AND UPPER(t.DS_TIPOCARTEIRA ) LIKE UPPER(:tipoCarteiraDescricao) ";
                    tipoCarteiraDescricao = String.Format("%{0}%", tipoCarteiraDescricao);
                    parameters.Add(new OracleParameter("tipoCarteiraDescricao", tipoCarteiraDescricao));
                }

                query_ += " ORDER BY Descricao ";

                ListaTipoCarteira = Context.ExecuteStoreQuery<TipoCarteiraDTO>(query_, parameters.ToArray()).ToList();

                if (ListaTipoCarteira != null)
                {
                    ListaTipoCarteira.ForEach(resu =>
                    {
                        resu.Programas.AddRange(PesquisarAssociacaoTipoCarteiraPrograma(resu.IdTipoCarteira));
                        resu.GrupoCategorias.AddRange(PesquisarAssociacaoTipoCarteiraGrupoCategoria(resu.IdTipoCarteira));
                    });
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "PesquisarTiposCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }


            return ListaTipoCarteira;
        }
        /// <summary>
        /// Pesquisa a Associação do tipo de carteira com os programas
        /// </summary>
        /// <param name="IdCarteira"></param>
        /// <returns>Lista de Programas</returns>
        public List<TipoCarteiraProgramaDTO> PesquisarAssociacaoTipoCarteiraPrograma(Int64 IdTipoCarteira)
        {
            String _querySQL = string.Empty;
            List<OracleParameter> parameters = null;
            List<TipoCarteiraProgramaDTO> ProgramasTipoCarteira = null;
            List<ProgramaConfigDTO> Programas = null;

            try
            {
                parameters = new List<OracleParameter>();
                _querySQL = @"
                        SELECT 
                            P.NM_PROGRAMA Nome,
                            TPP.ID_TIPOCARTEIRA_PROGRAMA IdCarteiraPrograma,
			                TPP.ID_PROGRAMA IdPrograma
                        FROM AMESP.GCP_TIPOCARTEIRA_PROGRAMA TPP
                            JOIN AMESP.CM_PROGRAMAS P
                                ON TPP.ID_PROGRAMA = P.ID_PROGRAMA
                        WHERE TPP.ID_TIPOCARTEIRA = :ID_TIPO_CARTEIRA";

                parameters.Add(new OracleParameter("ID_TIPO_CARTEIRA", IdTipoCarteira));
                Programas = Context.ExecuteStoreQuery<ProgramaConfigDTO>(_querySQL, parameters.ToArray()).ToList();

                if (Programas != null)
                {
                    ProgramasTipoCarteira = new List<TipoCarteiraProgramaDTO>(Programas.Count);
                    for (int i = 0; i < Programas.Count; i++)
                    {
                        ProgramasTipoCarteira.Add(new TipoCarteiraProgramaDTO
                        {
                            Programa = Programas[i],
                            IdCarteiraPrograma = IdTipoCarteira,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "PesquisarAssociacaoTipoCarteiraPrograma", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }
            return ProgramasTipoCarteira;
        }
        /// <summary>
        /// Pesquisa a Associação do tipo de carteira com os grupos de categorias
        /// </summary>
        /// <param name="IdCarteira"></param>
        /// <returns>Lista de Grupo de Categorias</returns>
        public List<TipoCarteiraGrupoCategoriaDTO> PesquisarAssociacaoTipoCarteiraGrupoCategoria(Int64 IdTipoCarteira)
        {
            String _querySQL = string.Empty;
            List<OracleParameter> parameters = null;
            List<TipoCarteiraGrupoCategoriaDTO> GrupoCategoriasTipoCarteira = null;
            try
            {
                parameters = new List<OracleParameter>();
                _querySQL = @"
                        select GC.ID_TIPOCARTEIRA_GRPCATEGORIA IdCarteiraGrupoCategoria,
                               GC.TP_PAPEL Profissional ,
			                   GC.ID_GRUPO_CATEGORIA IdGrupo,
			                   GR.DESCRICAO_GRUPO Nome 
                            FROM AMESP.GCP_TIPOCARTEIRA_GRPCATEGORIA GC
                            JOIN AMESP.CM_GRUPO_CATEGORIA_ATD GR
		                        ON GC.ID_GRUPO_CATEGORIA = GR.ID_GRUPO_CATEGORIA_ATD
		                        WHERE GC.ID_TIPOCARTEIRA = :ID_TIPO_CARTEIRA";

                parameters.Add(new OracleParameter("ID_TIPO_CARTEIRA", IdTipoCarteira));
                GrupoCategoriasTipoCarteira = Context.ExecuteStoreQuery<TipoCarteiraGrupoCategoriaDTO>(_querySQL, parameters.ToArray()).ToList();

                if (GrupoCategoriasTipoCarteira != null)
                {
                    for (int i = 0; i < GrupoCategoriasTipoCarteira.Count; i++)
                    {
                        GrupoCategoriasTipoCarteira[i].GrupoCategoria = new CategoriaGrupoDTO
                        {
                            GrupoCategoriaID = GrupoCategoriasTipoCarteira[i].IdGrupo,
                            NomeGrupo = GrupoCategoriasTipoCarteira[i].Nome
                        };

                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "PesquisarAssociacaoTipoCarteiraGrupoCategoria", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }
            return GrupoCategoriasTipoCarteira;
        }
        /// <summary>
        /// Pesquisar um tipo de categoria com base em uma ID.
        /// </summary>
        /// <param name="IdTipoCarteira"></param>
        /// <returns>Registro unico de tipo de carteira</returns>
        public TipoCarteiraDTO ObterTipoCarteiraPorId(Int64 IdTipoCarteira)
        {
            string query_ = string.Empty;
            List<OracleParameter> parameters = null;
            TipoCarteiraDTO TipoCarteira = null;

            try
            {
                parameters = new List<OracleParameter>();
                query_ = (@" SELECT  
                                t.ID_TIPOCARTEIRA IdTipoCarteira,
                                t.DS_TIPOCARTEIRA Descricao,
                                t.CD_TIPOCARTEIRA Codigo,
                                t.ST_ATIVO Ativo,
                                t.FL_MEDICO FlagMedico,
                                t.FL_ENFERMEIRO FlagEnfermeiro,
                                t.FL_AGENTESAUDE FlagAgenteSaude,
                                t.IND_GRUPO_INDIV IndicadorGrpIndividual,
                                t.FL_ATIVA_LIMITE AtivaLimite,
                                t.TP_LIMITE  TipoLimite,
                                t.FL_HABILITA_LISTA_CONTATOS HabilitaListaContato,
                                t.QT_MAX_CONTATOS QtMaximaContato,
                                t.QT_TENTATIVAS_DIA QtTentativasDia,
                                t.QT_DIAS_REAGENDAR QtDiasReagendar,
                                t.QT_DIAS_RETORNO QtDiasRetorno,
                                t.FL_SINALIZA_PACIENTE FlagSinalizaPaciente,
                                t.FL_PERMITE_ASSOCIACAO FlagPermiteAssociacao,
                                t.FL_PERMITE_CAPTACAO FlagPermiteCaptacao,
                                t.FL_STANDBY FlagStandBy,
                                t.Fl_Aprazar_Gc FlagAprazar,
                                t.FL_ESPECIALIDADE FlagEspecialidade,
                                t.FL_PERMITE_EXCLUSAO_CARTEIRA FlagPermissaoExclusao,
                                t.DT_CRIACAO DataCriacao,
                                t.USR_CRIACAO UsuarioCriacao,
                                t.DT_ULT_ALTERACAO DataUltimaAlteracao,
                                t.USR_ULT_ALTERACAO UsuarioUltAlteracao,
                                t.FL_PERMITE_EDICAO_PROGRAMA FlagPermissaoEdicao
                             FROM AMESP.GCP_TIPOCARTEIRA t 
                             WHERE t.id_tipocarteira = :IdCarteira ");

                parameters.Add(new OracleParameter("IdCarteira", IdTipoCarteira));
                TipoCarteira = Context.ExecuteStoreQuery<TipoCarteiraDTO>(query_, parameters.ToArray()).ToList().FirstOrDefault();

                if (TipoCarteira != null)
                {
                    TipoCarteira.Programas.AddRange(PesquisarAssociacaoTipoCarteiraPrograma(TipoCarteira.IdTipoCarteira));
                    TipoCarteira.GrupoCategorias.AddRange(PesquisarAssociacaoTipoCarteiraGrupoCategoria(TipoCarteira.IdTipoCarteira));

                }

                return TipoCarteira;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "ObterTipoCarteiraPorId", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }

        }
        /// <summary>
        /// Incluir um relacionamento entre tipo de carteira x Grupo de Categoria
        /// </summary>
        /// <param name="IdTipoCarteira"></param>
        /// <param name="IdGrupoCategoria"></param>
        /// <param name="Profissional"></param>
        /// <returns>Sucesso/Falha</returns>
        public bool IncluirTipoCarteiraGrupoCategoria(Int64 IdTipoCarteira, Int64 IdGrupoCategoria, string Profissional)
        {
            string query_ = string.Empty;
            OracleParameter[] parameters = null;
            bool resultado = false;

            try
            {
                parameters = new OracleParameter[3];
                query_ = (@" INSERT INTO AMESP.GCP_TIPOCARTEIRA_GRPCATEGORIA  
                                (ID_TIPOCARTEIRA,
                                 TP_PAPEL,
                                 ID_GRUPO_CATEGORIA)
                            VALUES(:ID_TIPOCARTEIRA,
                                   :PROFISSIONAL,
                                   :ID_GRUPO)");

                parameters[0] = new OracleParameter("ID_TIPOCARTEIRA", IdTipoCarteira);
                parameters[1] = new OracleParameter("PROFISSIONAL", Profissional);
                parameters[2] = new OracleParameter("ID_GRUPO", IdGrupoCategoria);

                Context.ExecuteStoreCommand(query_, parameters);
                resultado = true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "IncluirTipoCarteiraGrupoCategoria", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }

            return resultado;
        }
        /// <summary>
        /// Excluir um relacionamento entre tipo de carteira x Grupo de Categoria
        /// </summary>
        /// <param name="IdTipoCarteira"></param>
        /// <param name="IdGrupoCategoria"></param>
        /// <param name="Profissional"></param>
        /// <returns>Sucesso/Falha</returns>
        public bool ExcluirTipoCarteiraGrupoCategoria(Int64 IdTipoCategoriaGrupoCategoria)
        {
            string query_ = string.Empty;
            OracleParameter[] parameters = null;
            bool resultado = false;
            try
            {
                parameters = new OracleParameter[1];
                query_ = (@" DELETE FROM AMESP.GCP_TIPOCARTEIRA_GRPCATEGORIA  
                            WHERE ID_TIPOCARTEIRA_GRPCATEGORIA = :ID_TIPOCAT_GRUPOCATEGORIA");

                parameters[0] = new OracleParameter("ID_TIPOCAT_GRUPOCATEGORIA", IdTipoCategoriaGrupoCategoria);

                Context.ExecuteStoreCommand(query_, parameters);
                resultado = true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "ExcluirTipoCarteiraGrupoCategoria", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }


            return resultado;
        }
        /// <summary>
        /// Incluir um relacionamento entre tipo de carteira x Programas
        /// </summary>
        /// <param name="IdTipoCarteira"></param>
        /// <param name="IdPrograma"></param>
        /// <returns>Sucesso/Falha</returns>
        public bool IncluirTipoCarteiraPrograma(Int64 IdTipoCarteira, Int64 IdPrograma)
        {
            string query_ = string.Empty;
            OracleParameter[] parameters = null;
            bool resultado = false;

            try
            {
                parameters = new OracleParameter[2];
                query_ = (@" INSERT INTO AMESP.GCP_TIPOCARTEIRA_PROGRAMA (ID_TIPOCARTEIRA, ID_PROGRAMA)
                            VALUES(:ID_TIPOCARTEIRA, :ID_PROGRAMA)");

                parameters[0] = new OracleParameter("ID_TIPOCARTEIRA", IdTipoCarteira);
                parameters[1] = new OracleParameter("ID_PROGRAMA", IdPrograma);

                Context.ExecuteStoreCommand(query_, parameters);
                resultado = true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "IncluirTipoCarteiraPrograma", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }


            return resultado;
        }
        /// <summary>
        /// Excluir um relacionamento entre tipo de carteira x programas
        /// </summary>
        /// <param name="IdTipoCarteira"></param>
        /// <param name="IdPrograma"></param>
        /// <returns>sucesso/falha</returns>
        public bool ExcluirirTipoCarteiraPrograma(Int64 IdTipoCarteira, Int64 IdPrograma)
        {
            string query_ = string.Empty;
            OracleParameter[] parameters = null;
            bool resultado = false;
            try
            {
                parameters = new OracleParameter[2];
                query_ = (@" DELETE FROM AMESP.GCP_TIPOCARTEIRA_PROGRAMA  
                          WHERE ID_TIPOCARTEIRA = :ID_TIPOCARTEIRA
                            AND ID_PROGRAMA = :ID_PROGRAMA");


                parameters[0] = new OracleParameter("ID_TIPOCARTEIRA", IdTipoCarteira);
                parameters[1] = new OracleParameter("ID_PROGRAMA", IdPrograma);

                Context.ExecuteStoreCommand(query_, parameters);
                resultado = true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "ExcluirirTipoCarteiraPrograma", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }


            return resultado;
        }



        /// <summary>
        /// Realiza Cadastro de um novo registro de um tipo de carteira
        /// </summary>
        /// <param name="objTipoCarteira"></param>
        /// <returns>flag de sucesso/falha</returns>
        public Boolean IncluirTipoCarteira(TipoCarteiraDTO objTipoCarteira)
        {
            StringBuilder query = null;
            bool _return = false;
            Int64 IdCarteira = 0;

            try
            {
                query = new StringBuilder();
                query.Append(@"INSERT INTO AMESP.GCP_TIPOCARTEIRA
                    (
                        DS_TIPOCARTEIRA, 
                        CD_TIPOCARTEIRA, 
                        ST_ATIVO, 
                        FL_MEDICO, 
                        FL_ENFERMEIRO,
                        FL_AGENTESAUDE, 
                        IND_GRUPO_INDIV, 
                        FL_ATIVA_LIMITE, 
                        TP_LIMITE, 
                        FL_HABILITA_LISTA_CONTATOS,
                        QT_MAX_CONTATOS, 
                        QT_TENTATIVAS_DIA, 
                        QT_DIAS_REAGENDAR, 
                        QT_DIAS_RETORNO, 
                        FL_SINALIZA_PACIENTE,
                        FL_PERMITE_ASSOCIACAO, 
                        FL_PERMITE_CAPTACAO, 
                        FL_STANDBY,
                        FL_APRAZAR_GC,
                        FL_ESPECIALIDADE, 
                        FL_PERMITE_EXCLUSAO_CARTEIRA,
                        DT_CRIACAO, 
                        USR_CRIACAO, 
                        DT_ULT_ALTERACAO, 
                        USR_ULT_ALTERACAO,
                        FL_PERMITE_EDICAO_PROGRAMA
                    ) ");
                query.Append(@" VALUES (:DS_TIPOCARTEIRA
                                       ,:CD_TIPOCARTEIRA 
                                       ,:ST_ATIVO
                                       ,:FL_MEDICO 
                                       ,:FL_ENFERMEIRO  
                                       ,:FL_AGENTESAUDE  
                                       ,:IND_GRUPO_INDIV  
                                       ,:FL_ATIVA_LIMITE  
                                       ,:TP_LIMITE  
                                       ,:FL_HABILITA_LISTA_CONTATOS  
                                       ,:QT_MAX_CONTATOS  
                                       ,:QT_TENTATIVAS_DIA  
                                       ,:QT_DIAS_REAGENDAR  
                                       ,:QT_DIAS_RETORNO  
                                       ,:FL_SINALIZA_PACIENTE  
                                       ,:FL_PERMITE_ASSOCIACAO  
                                       ,:FL_PERMITE_CAPTACAO  
                                       ,:FL_STANDBY
                                       ,:FL_APRAZAR_GC
                                       ,:FL_ESPECIALIDADE 
                                       ,:FL_PERMITE_EXCLUSAO_CARTEIRA
                                       ,:DT_CRIACAO
                                       ,:USR_CRIACAO 
                                       ,:DT_ULT_ALTERACAO 
                                       ,:USR_ULT_ALTERACAO
                                       ,:FL_PERMITE_EDICAO_PROGRAMA) 
                    RETURNING ID_TIPOCARTEIRA INTO :IdChave ");

                var parameters = new OracleParameter[27];
                parameters[0] = new OracleParameter("DS_TIPOCARTEIRA", objTipoCarteira.Descricao);
                parameters[1] = new OracleParameter("CD_TIPOCARTEIRA", objTipoCarteira.Codigo);
                parameters[2] = new OracleParameter("ST_ATIVO", objTipoCarteira.Ativo);
                parameters[3] = new OracleParameter("FL_MEDICO", objTipoCarteira.FlagMedico);
                parameters[4] = new OracleParameter("FL_ENFERMEIRO", objTipoCarteira.FlagEnfermeiro);
                parameters[5] = new OracleParameter("FL_AGENTESAUDE", objTipoCarteira.FlagAgenteSaude);
                parameters[6] = new OracleParameter("IND_GRUPO_INDIV", objTipoCarteira.IndicadorGrpIndividual);
                parameters[7] = new OracleParameter("FL_ATIVA_LIMITE", objTipoCarteira.AtivaLimite);
                parameters[8] = new OracleParameter("TP_LIMITE", objTipoCarteira.TipoLimite);
                parameters[9] = new OracleParameter("FL_HABILITA_LISTA_CONTATOS", objTipoCarteira.HabilitaListaContato);
                parameters[10] = new OracleParameter("QT_MAX_CONTATOS", objTipoCarteira.QtMaximaContato);
                parameters[11] = new OracleParameter("QT_TENTATIVAS_DIA", objTipoCarteira.QtTentativasDia);
                parameters[12] = new OracleParameter("QT_DIAS_REAGENDAR", objTipoCarteira.QtDiasReagendar);
                parameters[13] = new OracleParameter("QT_DIAS_RETORNO", objTipoCarteira.QtDiasRetorno);
                parameters[14] = new OracleParameter("FL_SINALIZA_PACIENTE", objTipoCarteira.FlagSinalizaPaciente);
                parameters[15] = new OracleParameter("FL_PERMITE_ASSOCIACAO", objTipoCarteira.FlagPermiteAssociacao);
                parameters[16] = new OracleParameter("FL_PERMITE_CAPTACAO", objTipoCarteira.FlagPermiteCaptacao);
                parameters[17] = new OracleParameter("FL_STANDBY", objTipoCarteira.FlagStandBy);
                parameters[18] = new OracleParameter("FL_APRAZAR_GC", objTipoCarteira.FlagAprazar);
                parameters[19] = new OracleParameter("FL_ESPECIALIDADE", objTipoCarteira.FlagEspecialidade);
                parameters[20] = new OracleParameter("FL_PERMITE_EXCLUSAO_CARTEIRA", objTipoCarteira.FlagPermissaoExclusao);
                parameters[21] = new OracleParameter("DT_CRIACAO", objTipoCarteira.DataCriacao);
                parameters[22] = new OracleParameter("USR_CRIACAO", objTipoCarteira.UsuarioCriacao);
                parameters[23] = new OracleParameter("DT_ULT_ALTERACAO", objTipoCarteira.DataUltimaAlteracao);
                parameters[24] = new OracleParameter("USR_ULT_ALTERACAO", objTipoCarteira.UsuarioUltAlteracao);
                parameters[25] = new OracleParameter("FL_PERMITE_EDICAO_PROGRAMA", objTipoCarteira.FlagPermissaoEdicao);

                parameters[26] = new OracleParameter("IdChave", OracleDbType.Int64);
                parameters[26].Direction = ParameterDirection.Output;

                var result = Context.ExecuteStoreCommand(query.ToString(), parameters);
                _return = true;
                if (_return)
                {
                    _return = false;
                    if (Int64.TryParse(parameters[26].Value.ToString(), out IdCarteira))
                    {
                        objTipoCarteira.GrupoCategorias.ForEach(item =>
                        {
                            this.IncluirTipoCarteiraGrupoCategoria(IdCarteira, item.GrupoCategoria.GrupoCategoriaID, item.Profissional);
                        });

                        objTipoCarteira.Programas.ForEach(item =>
                        {
                            this.IncluirTipoCarteiraPrograma(IdCarteira, item.Programa.IdPrograma);
                        });

                        _return = true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "IncluirTipoCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }

            return _return;
        }

        /// <summary>
        /// Atualiza os dados do tipo de carteira
        /// </summary>
        /// <param name="objTipoCarteira"></param>
        /// <returns>Verdade-Atualizou/Falso - Não atualizou</returns>
        public Boolean AtualizarTipoCarteira(TipoCarteiraDTO objTipoCarteira)
        {
            StringBuilder query = null;
            bool sucessoTipoCarteira = true;

            using (var transaction = TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    query = new StringBuilder();

                    #region Alterando o TipoCarteira
                    query.Append(@"UPDATE AMESP.GCP_TIPOCARTEIRA
                                SET DS_TIPOCARTEIRA                 = :DS_TIPOCARTEIRA
                                    ,CD_TIPOCARTEIRA                = :CD_TIPOCARTEIRA
                                    ,ST_ATIVO                       = :ST_ATIVO
                                    ,FL_MEDICO                      = :FL_MEDICO
                                    ,FL_ENFERMEIRO                  = :FL_ENFERMEIRO
                                    ,FL_AGENTESAUDE                 = :FL_AGENTESAUDE
                                    ,IND_GRUPO_INDIV                = :IND_GRUPO_INDIV
                                    ,FL_ATIVA_LIMITE                = :FL_ATIVA_LIMITE
                                    ,TP_LIMITE                      = :TP_LIMITE
                                    ,FL_HABILITA_LISTA_CONTATOS     = :FL_HABILITA_LISTA_CONTATOS
                                    ,QT_MAX_CONTATOS                = :QT_MAX_CONTATOS
                                    ,QT_TENTATIVAS_DIA              = :QT_TENTATIVAS_DIA
                                    ,QT_DIAS_REAGENDAR              = :QT_DIAS_REAGENDAR
                                    ,QT_DIAS_RETORNO                = :QT_DIAS_RETORNO
                                    ,FL_SINALIZA_PACIENTE           = :FL_SINALIZA_PACIENTE
                                    ,FL_PERMITE_ASSOCIACAO          = :FL_PERMITE_ASSOCIACAO
                                    ,FL_PERMITE_CAPTACAO            = :FL_PERMITE_CAPTACAO
                                    ,FL_STANDBY                     = :FL_STANDBY
                                    ,FL_APRAZAR_GC                  = :FL_APRAZAR_GC
                                    ,FL_ESPECIALIDADE               = :FL_ESPECIALIDADE
                                    ,FL_PERMITE_EXCLUSAO_CARTEIRA   = :FL_PERMISSAO_EXCLUSAO
                                    ,DT_CRIACAO                     = :DT_CRIACAO
                                    ,USR_CRIACAO                    = :USR_CRIACAO
                                    ,DT_ULT_ALTERACAO               = :DT_ULT_ALTERACAO
                                    ,USR_ULT_ALTERACAO              = :USR_ULT_ALTERACAO 
                                    ,FL_PERMITE_EDICAO_PROGRAMA     = :FL_PERMITE_EDICAO_PROGRAMA
                                WHERE ID_TIPOCARTEIRA               = :ID_TIPOCARTEIRA ");

                    var parameters = new OracleParameter[27];
                    parameters[0] = new OracleParameter("DS_TIPOCARTEIRA", objTipoCarteira.Descricao);
                    parameters[1] = new OracleParameter("CD_TIPOCARTEIRA", objTipoCarteira.Codigo);
                    parameters[2] = new OracleParameter("ST_ATIVO", objTipoCarteira.Ativo);
                    parameters[3] = new OracleParameter("FL_MEDICO", objTipoCarteira.FlagMedico);
                    parameters[4] = new OracleParameter("FL_ENFERMEIRO", objTipoCarteira.FlagEnfermeiro);
                    parameters[5] = new OracleParameter("FL_AGENTESAUDE", objTipoCarteira.FlagAgenteSaude);
                    parameters[6] = new OracleParameter("IND_GRUPO_INDIV", objTipoCarteira.IndicadorGrpIndividual);
                    parameters[7] = new OracleParameter("FL_ATIVA_LIMITE", objTipoCarteira.AtivaLimite);
                    parameters[8] = new OracleParameter("TP_LIMITE", (objTipoCarteira.AtivaLimite.HasValue ? objTipoCarteira.AtivaLimite.Value ? objTipoCarteira.TipoLimite : null : null));
                    parameters[9] = new OracleParameter("FL_HABILITA_LISTA_CONTATOS", objTipoCarteira.HabilitaListaContato);
                    parameters[10] = new OracleParameter("QT_MAX_CONTATOS", objTipoCarteira.QtMaximaContato);
                    parameters[11] = new OracleParameter("QT_TENTATIVAS_DIA", objTipoCarteira.QtTentativasDia);
                    parameters[12] = new OracleParameter("QT_DIAS_REAGENDAR", objTipoCarteira.QtDiasReagendar);
                    parameters[13] = new OracleParameter("QT_DIAS_RETORNO", objTipoCarteira.QtDiasRetorno);
                    parameters[14] = new OracleParameter("FL_SINALIZA_PACIENTE", objTipoCarteira.FlagSinalizaPaciente);
                    parameters[15] = new OracleParameter("FL_PERMITE_ASSOCIACAO", objTipoCarteira.FlagPermiteAssociacao);
                    parameters[16] = new OracleParameter("FL_PERMITE_CAPTACAO", objTipoCarteira.FlagPermiteCaptacao);
                    parameters[17] = new OracleParameter("FL_ESPECIALIDADE", objTipoCarteira.FlagEspecialidade);
                    parameters[18] = new OracleParameter("DT_CRIACAO", objTipoCarteira.DataCriacao);
                    parameters[19] = new OracleParameter("USR_CRIACAO", objTipoCarteira.UsuarioCriacao);
                    parameters[20] = new OracleParameter("DT_ULT_ALTERACAO", objTipoCarteira.DataUltimaAlteracao);
                    parameters[21] = new OracleParameter("USR_ULT_ALTERACAO", objTipoCarteira.UsuarioUltAlteracao);
                    parameters[22] = new OracleParameter("FL_PERMISSAO_EXCLUSAO", objTipoCarteira.FlagPermissaoExclusao);
                    parameters[23] = new OracleParameter("FL_STANDBY", objTipoCarteira.FlagStandBy);
                    parameters[24] = new OracleParameter("FL_APRAZAR_GC", objTipoCarteira.FlagAprazar);
                    parameters[25] = new OracleParameter("ID_TIPOCARTEIRA", objTipoCarteira.IdTipoCarteira);
                    parameters[26] = new OracleParameter("FL_PERMITE_EDICAO_PROGRAMA", objTipoCarteira.FlagPermissaoEdicao);

                    sucessoTipoCarteira = Context.ExecuteStoreCommand(query.ToString(), parameters) > 0;

                    #endregion

                    #region Alterando limites
                    // Regra de Negocio para tipo de limite alterado.
                    // objTipoCarteira.TipoLimiteAlterado { Indica que houve alteração no tipo de limite (Modificado de Quantidade para Pontuação ou vice-versa).
                    // objTipoCarteira.LimiteCarteiraModificado { Indica se foi habilitado ou desabilitado para que o tipo de carteira use o tipo de limite).
                    if (objTipoCarteira.LimiteCarteiraModificado || objTipoCarteira.TipoLimiteAlterado)
                    {
                        var AtivarLimite = (objTipoCarteira.AtivaLimite.HasValue ? objTipoCarteira.AtivaLimite.Value : false);
                        var tipoIndividual = !string.IsNullOrEmpty(objTipoCarteira.IndicadorGrpIndividual) && objTipoCarteira.IndicadorGrpIndividual == "I";
                        var listaProfissionaisLimites = ObterValorLimiteProfissionalPorTipoCarteira(objTipoCarteira.IdTipoCarteira, objTipoCarteira.TipoLimite, tipoIndividual);

                        if (listaProfissionaisLimites.Any())
                        {
                            listaProfissionaisLimites.ForEach(item =>
                            {
                                if (item.CarteiraProfissionalID.HasValue)
                                    this.AjustarLimiteCarteira(item.CarteiraProfissionalID.Value, AtivarLimite, item.LimiteQuantidadeTotal);
                            });
                        }
                    }
                    #endregion

                    #region Alterando programas vinculados
                    if (objTipoCarteira.Programas != null && objTipoCarteira.Programas.Count > 0)
                    {
                        objTipoCarteira.Programas.ForEach(item =>
                        {
                            if (item.ProgramaExcluido)
                                this.ExcluirirTipoCarteiraPrograma(objTipoCarteira.IdTipoCarteira, item.Programa.IdPrograma);
                            else
                                this.IncluirTipoCarteiraPrograma(objTipoCarteira.IdTipoCarteira, item.Programa.IdPrograma);
                        });
                    }
                    #endregion

                    #region Alterando grupos de categorias vinculados
                    if (objTipoCarteira.GrupoCategorias != null && objTipoCarteira.GrupoCategorias.Count > 0)
                    {
                        objTipoCarteira.GrupoCategorias.ForEach(item =>
                        {
                            if (item.ExcluirGrupoCategoria)
                                this.ExcluirTipoCarteiraGrupoCategoria(item.IdCarteiraGrupoCategoria);
                            else
                                this.IncluirTipoCarteiraGrupoCategoria(objTipoCarteira.IdTipoCarteira, item.GrupoCategoria.GrupoCategoriaID, item.Profissional);
                        });

                    }
                    #endregion

                    transaction.Complete();
                    return sucessoTipoCarteira;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(this._mensagemErro, "AtualizarTipoCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
                }
            }
        }

        public bool AjustarLimiteCarteira(Int64 CarteiraProfissionalID, bool ativarLimite, decimal? novoValor)
        {
            try
            {
                var query = new StringBuilder();

                if (!ativarLimite)
                    novoValor = null;

                query.Append(@"
                    UPDATE AMESP.GCP_CARTEIRA_PROFISSIONAL
                    SET VL_LIMITE = :VALOR
                    WHERE 
                        ID_CARTEIRA_PROFISSIONAL = :CarteiraProfissionalID ");

                var parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("VALOR", novoValor));
                parameters.Add(new OracleParameter("CarteiraProfissionalID", CarteiraProfissionalID));


                var qtdeItemsAlterados = Context.ExecuteStoreCommand(query.ToString(), parameters.ToArray());
                return qtdeItemsAlterados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "AjustarLimiteCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }
        }

        public List<TipoCarteiraLimiteTotalCarteira> ObterValorLimiteProfissionalPorTipoCarteira(long TipoCarteiraID, string tipoLimite, bool carteiraIndividual)
        {
            StringBuilder _query = null;
            List<OracleParameter> parametros = null;

            try
            {
                _query = new StringBuilder();
                parametros = new List<OracleParameter>();

                _query.Append(@"
                    SELECT
                        CP.ID_CARTEIRA_PROFISSIONAL AS CarteiraProfissionalID,
                        CP.ID_TIPOCARTEIRA AS TipoCarteiraID,
                        CP.ID_PROFISSIONAL AS IdProfissional,
                        CP.ID_EQUIPE AS EquipeSaudeID,
                        CP.TP_PAPEL AS TipoPapelProfissional,
                        NVL ({0}, 0) AS LimiteQuantidadeTotal
                    FROM AMESP.GCP_CARTEIRA_PROFISSIONAL CP
                    WHERE
                       CP.ID_TIPOCARTEIRA = :TipoCarteiraID              
                ");

                string _queryLimites = string.Empty;
                if (tipoLimite == "Q")
                {
                    _queryLimites = @"(
                        SELECT COUNT (CPE.ID_PESSOA)
                        FROM AMESP.GCP_CARTEIRA_PESSOA CPE
                        WHERE CPE.ID_TIPOCARTEIRA = CP.ID_TIPOCARTEIRA 
                            AND CPE.ID_PROFISSIONAL = CP.ID_PROFISSIONAL
                            AND CPE.ID_EQUIPE " + 
                        (carteiraIndividual ? " IS NULL " : " = CP.ID_EQUIPE ") + 
                    ")";
                }
                else if (tipoLimite == "P")
                {
                    _queryLimites = @"(
                        SELECT SUM (
                            (SELECT CCP.VL_FAIXA
                            FROM AMESP.GLB_PESSOA_FISICA PF
                            JOIN AMESP.CONFIG_CARTEIRA_PONT CCP
                                ON (CCP.FL_GENERO = PF.ID_SEXO AND AMESP.CALC_IDADE(PF.DT_NASCIMENTO, SYSDATE) 
                                BETWEEN CCP.VL_IDADE_INI AND CCP.VL_IDADE_FIM)
                            WHERE PF.ID_PESSOA = CPE.ID_PESSOA)
                        )
                        FROM AMESP.GCP_CARTEIRA_PESSOA CPE
                        WHERE CPE.ID_TIPOCARTEIRA = CP.ID_TIPOCARTEIRA 
                            AND CPE.ID_PROFISSIONAL = CP.ID_PROFISSIONAL
                            AND CPE.ID_EQUIPE " +
                        (carteiraIndividual ? " IS NULL " : " = CP.ID_EQUIPE ") +
                    ")";
                }
                else
                {
                    _queryLimites = "0";
                }

                parametros.Add(new OracleParameter("TipoCarteiraID", TipoCarteiraID));
                string _queryFinal = string.Format(_query.ToString(), _queryLimites);

                return Context.ExecuteStoreQuery<TipoCarteiraLimiteTotalCarteira>(_queryFinal, parametros.ToArray()).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obsoleto. Usar o método ObterValorLimiteProfissionalPorTipoCarteira
        /// </summary>
        public List<TipoCarteiraLimiteTotalCarteira> ObterLimiteTotalCarteira(Int64 IdTipoCarteira, string TipoProfissionais)
        {
            String _querySQL = string.Empty;
            List<OracleParameter> parameters = null;
            List<TipoCarteiraLimiteTotalCarteira> ListaLimiteTotal = null;
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
			                    else 0 END) LimiteQuantidadeTotal
	                        from amesp.gcp_carteira_pessoa md 
		                        where md.id_tipocarteira =:ID_TIPO_CARTEIRA
		                          AND md.tp_papel IN({0})
                        GROUP BY md.id_profissional", TipoProfissionais);

                parameters.Add(new OracleParameter("ID_TIPO_CARTEIRA", IdTipoCarteira));
                ListaLimiteTotal = Context.ExecuteStoreQuery<TipoCarteiraLimiteTotalCarteira>(_querySQL, parameters.ToArray()).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "ObterLimiteTotalCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }

            return ListaLimiteTotal;
        }

        /// <summary>
        /// Obsoleto. Usar o método ObterValorLimiteProfissionalPorTipoCarteira
        /// </summary>
        public List<TipoCarteiraLimiteTotalCarteira> ObterQuantidadeTotalCarteira(Int64 IdTipoCarteira, string TipoProfissionais)
        {
            String _querySQL = string.Empty;
            List<OracleParameter> parameters = null;
            List<TipoCarteiraLimiteTotalCarteira> ListaLimiteTotal = null;
            try
            {
                parameters = new List<OracleParameter>();
                _querySQL = string.Format(@"
                         select 
                          md.id_profissional IdProfissional,
                        count(ID_PESSOA) LimiteQuantidadeTotal
	                        from amesp.gcp_carteira_pessoa md 
                         where md.id_tipocarteira =:ID_TIPO_CARTEIRA
		                          AND md.tp_papel IN({0})
                        GROUP BY md.id_profissional", TipoProfissionais);

                parameters.Add(new OracleParameter("ID_TIPO_CARTEIRA", IdTipoCarteira));
                ListaLimiteTotal = Context.ExecuteStoreQuery<TipoCarteiraLimiteTotalCarteira>(_querySQL, parameters.ToArray()).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(this._mensagemErro, "ObterQuantidadeTotalCarteira", ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }

            return ListaLimiteTotal;
        }

        /// <summary>
        /// Pesquisa os tipos de carteira vinculados a carteira de um determinado profissional;
        /// </summary>
        /// <param name="idpProfissional">Id do profissional</param>
        /// <returns>Lista de tipo de carteira</returns>
        public List<TipoCarteiraDTO> PesquisarTiposCarteirasPorProfissional(long ProfissionalID)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@" SELECT DISTINCT                
                            tc.ID_TIPOCARTEIRA as IdTipoCarteira,
                            tc.DS_TIPOCARTEIRA as Descricao,
                            tc.CD_TIPOCARTEIRA as Codigo,
                            tc.ST_ATIVO as Ativo,
                            tc.FL_MEDICO as FlagMedico,
                            tc.FL_ENFERMEIRO as FlagEnfermeiro,
                            tc.FL_AGENTESAUDE as FlagAgenteSaude,
                            tc.IND_GRUPO_INDIV as IndicadorGrpIndividual,
                            tc.FL_ATIVA_LIMITE as AtivaLimite,
                            tc.TP_LIMITE as TipoLimite,
                            tc.FL_HABILITA_LISTA_CONTATOS as HabilitaListaContato,
                            tc.QT_MAX_CONTATOS as QtMaximaContato,
                            tc.QT_TENTATIVAS_DIA as QtTentativasDia,
                            tc.QT_DIAS_REAGENDAR as QtDiasReagendar,
                            tc.QT_DIAS_RETORNO as QtDiasRetorno,
                            tc.FL_SINALIZA_PACIENTE as FlagSinalizaPaciente,
                            tc.FL_PERMITE_ASSOCIACAO as FlagPermiteAssociacao,
                            tc.FL_PERMITE_CAPTACAO as FlagPermiteCaptacao,
                            tc.FL_ESPECIALIDADE as FlagEspecialidade,
                            tc.FL_PERMITE_EXCLUSAO_CARTEIRA as FlagPermissaoExclusao,
                            tc.DT_CRIACAO as DataCriacao,
                            tc.USR_CRIACAO as UsuarioCriacao,
                            tc.DT_ULT_ALTERACAO as DataUltimaAlteracao,
                            tc.USR_ULT_ALTERACAO as UsuarioUltAlteracao,
                            tc.FL_PERMITE_EDICAO_PROGRAMA as FlagPermissaoEdicao
                            FROM amesp.gcp_carteira_profissional cp
                            INNER JOIN amesp.gcp_tipocarteira tc ON tc.id_tipocarteira = cp.id_tipocarteira
                            WHERE cp.id_profissional = :ProfissionalID");

            parameters.Add(new OracleParameter("ProfissionalID", ProfissionalID));
            return Context.ExecuteStoreQuery<TipoCarteiraDTO>(query.ToString(), parameters.ToArray()).ToList();
        }

        public bool VerificarFlagContatoHabilitado(int idTipoCarteira)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@" select t.fl_habilita_lista_contatos from amesp.gcp_tipocarteira t where t.id_tipocarteira = :idTipoCarteira");

            parameters.Add(new OracleParameter("idTipoCarteira", idTipoCarteira));

            var flag = Context.ExecuteStoreQuery<int>(query.ToString(), parameters.ToArray()).FirstOrDefault();

            if (flag > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Pesquisa os tipos de carteira vinculados a carteira de um determinado profissional na unidade de atendimento;
        /// </summary>
        /// <param name="idpProfissional">Id do profissional</param>
        /// <param name="CentroMedico">Unidade de atendimento</param>
        /// <returns>Lista de tipo de carteira</returns>
        public List<TipoCarteiraDTO> PesquisarTiposCarteiraCaptacaoPEP(long ProfissionalID, string CentroMedico, bool somentePermiteCaptacao = true)
        {
            StringBuilder query = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            query.Append(@" SELECT DISTINCT
                            tc.ID_TIPOCARTEIRA as IdTipoCarteira,
                            tc.DS_TIPOCARTEIRA as Descricao,
                            tc.CD_TIPOCARTEIRA as Codigo,
                            tc.ST_ATIVO as Ativo,
                            tc.FL_MEDICO as FlagMedico,
                            tc.FL_ENFERMEIRO as FlagEnfermeiro,
                            tc.FL_AGENTESAUDE as FlagAgenteSaude,
                            tc.IND_GRUPO_INDIV as IndicadorGrpIndividual,
                            tc.FL_ATIVA_LIMITE as AtivaLimite,
                            tc.TP_LIMITE as TipoLimite,
                            tc.FL_HABILITA_LISTA_CONTATOS as HabilitaListaContato,
                            tc.QT_MAX_CONTATOS as QtMaximaContato,
                            tc.QT_TENTATIVAS_DIA as QtTentativasDia,
                            tc.QT_DIAS_REAGENDAR as QtDiasReagendar,
                            tc.QT_DIAS_RETORNO as QtDiasRetorno,
                            tc.FL_SINALIZA_PACIENTE as FlagSinalizaPaciente,
                            tc.FL_PERMITE_ASSOCIACAO as FlagPermiteAssociacao,
                            tc.FL_PERMITE_CAPTACAO as FlagPermiteCaptacao,
                            tc.FL_ESPECIALIDADE as FlagEspecialidade,
                            tc.DT_CRIACAO as DataCriacao,
                            tc.USR_CRIACAO as UsuarioCriacao,
                            tc.DT_ULT_ALTERACAO as DataUltimaAlteracao,
                            tc.USR_ULT_ALTERACAO as UsuarioUltAlteracao,
                            tc.FL_PERMITE_EDICAO_PROGRAMA as FlagPermissaoEdicao
                            FROM amesp.gcp_carteira_profissional cp
                            INNER JOIN amesp.gcp_tipocarteira tc ON tc.id_tipocarteira = cp.id_tipocarteira
                            LEFT JOIN amesp.equipe_familia EF ON EF.ID_EQUIPE = CP.ID_EQUIPE
                            LEFT JOIN amesp.cm_unidade U ON U.CENTRO_MEDICO = EF.ID_UNIDADE
                            WHERE cp.id_profissional = :ProfissionalID
                            AND (u.CENTRO_MEDICO IS NULL OR u.CENTRO_MEDICO = :CentroMedico)
                            AND (cp.TP_PAPEL = 'M' OR cp.TP_PAPEL = 'E')");
            if (somentePermiteCaptacao)
            {
                query.Append(" AND tc.FL_PERMITE_CAPTACAO = 1");
            }
            query.Append(" AND tc.ST_ATIVO = 1");

            parameters.Add(new OracleParameter("ProfissionalID", ProfissionalID));
            parameters.Add(new OracleParameter("CentroMedico", CentroMedico));

            return Context.ExecuteStoreQuery<TipoCarteiraDTO>(query.ToString(), parameters.ToArray()).ToList();
        }

    }
}