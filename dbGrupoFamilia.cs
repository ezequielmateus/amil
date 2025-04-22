using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DTO;
using Oracle.DataAccess.Client;

namespace DAL
{
    public class dbGrupoFamilia
    {
        public List<tpGrupoFamilia> Listar(string idUnidade = null)
        {
            List<tpGrupoFamilia> lstGrupoFamilia = null;
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append(@"SELECT ID_GRUPO, ID_UNIDADE, NM_GRUPO, DT_CRIACAO, DT_EXCLUSAO FROM AMESP.GRUPO_FAMILIA");
                sql.Append(!string.IsNullOrEmpty(idUnidade) ? " WHERE ID_UNIDADE = '{0}'" : "");

                var dr = OracleHelper.ExecuteReader(sql.ToString());

                if (dr.HasRows)
                {
                    lstGrupoFamilia = new List<tpGrupoFamilia>();
                    while (dr.Read())
                    {
                        var entidade = new tpGrupoFamilia();

                        entidade.Id = dr["ID_GRUPO"].VerificaNuloInt();
                        entidade.CodigoUnidade = dr["ID_UNIDADE"].VerificaNuloString();
                        entidade.Nome = dr["NM_GRUPO"].VerificaNuloString();
                        entidade.Criacao = dr["DT_CRIACAO"].VerificaNuloDateTime();
                        entidade.Exclusao = dr["DT_EXCLUSAO"].VerificaNuloDateTime();

                        lstGrupoFamilia.Add(entidade);
                    }
                }

                dr.Close();

                return lstGrupoFamilia;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<tpGrupoFamilia> ConsultarPorUnidade(string idUnidade)
        {
            List<tpGrupoFamilia> lstGrupoFamilia = null;

            try
            {
                var sql = @"SELECT ID_GRUPO, ID_UNIDADE, NM_GRUPO, DT_CRIACAO, DT_EXCLUSAO FROM AMESP.GRUPO_FAMILIA
                             WHERE ID_UNIDADE = :ID_UNIDADE AND DT_EXCLUSAO IS NULL";

                var param = new OracleParameter[1];
                param[0] = new OracleParameter(":ID_UNIDADE", idUnidade)
                {
                    OracleDbType = OracleDbType.Varchar2
                };

                var dr = OracleHelper.ExecuteReader(sql, param);

                if (dr.HasRows)
                {
                    lstGrupoFamilia = new List<tpGrupoFamilia>();
                    while (dr.Read())
                    {
                        var entidade = new tpGrupoFamilia();

                        entidade.Id = dr["ID_GRUPO"].VerificaNuloInt();
                        entidade.CodigoUnidade = dr["ID_UNIDADE"].VerificaNuloString();
                        entidade.Nome = dr["NM_GRUPO"].VerificaNuloString();
                        entidade.Criacao = dr["DT_CRIACAO"].VerificaNuloDateTime();
                        entidade.Exclusao = dr["DT_EXCLUSAO"].VerificaNuloDateTime();

                        lstGrupoFamilia.Add(entidade);
                    }
                }

                dr.Close();

                return lstGrupoFamilia;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Inserir(tpGrupoFamilia grupoFamilia)
        {
            int idGrupo;
            try
            {
                var sql = new StringBuilder();

                sql.AppendLine("INSERT INTO AMESP.GRUPO_FAMILIA(");
                sql.AppendLine("        ID_UNIDADE,");
                sql.AppendLine("        NM_GRUPO,");
                sql.AppendLine("        DT_CRIACAO)");
                sql.AppendLine(" VALUES (");
                sql.AppendLine("        :ID_UNIDADE,");
                sql.AppendLine("        :NM_GRUPO,");
                sql.AppendLine("        :DT_CRIACAO)");
                sql.AppendLine("  returning ID_GRUPO INTO :ID_GRUPO ");

                var param = new OracleParameter[4];
                param[0] = new OracleParameter(":ID_UNIDADE", grupoFamilia.CodigoUnidade)
                {
                    OracleDbType = OracleDbType.Int32
                };
                param[1] = new OracleParameter(":NM_GRUPO", grupoFamilia.Nome)
                {
                    OracleDbType = OracleDbType.Varchar2
                };
                param[2] = new OracleParameter(":DT_CRIACAO", System.DateTime.Now)
                {
                    OracleDbType = OracleDbType.Date
                };

                param[3] = new OracleParameter(":ID_GRUPO", OracleDbType.Int32, ParameterDirection.ReturnValue);

                OracleHelper.ExecuteNonQuery(sql.ToString(), param);

                string _return = param[3].Value.ToString();
                idGrupo = _return.VerificaNuloInt();
                grupoFamilia.Id = _return.VerificaNuloInt();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idGrupo;
        }

        public int Atualizar(tpGrupoFamilia grupoFamilia)
        {
            int qtdeRows = 0;

            try
            {
                var sql = new StringBuilder();

                sql.AppendLine("UPDATE AMESP.GRUPO_FAMILIA ");
                sql.AppendLine(" SET    ID_UNIDADE        = :ID_UNIDADE,");
                sql.AppendLine("        NM_GRUPO          = :NM_GRUPO");
                sql.AppendLine(" WHERE  ID_GRUPO          = :ID_GRUPO ");

                var param = new OracleParameter[3];

                param[0] = new OracleParameter(":ID_UNIDADE", grupoFamilia.CodigoUnidade)
                {
                    OracleDbType = OracleDbType.Varchar2
                };
                param[1] = new OracleParameter(":NM_GRUPO", grupoFamilia.Nome)
                {
                    OracleDbType = OracleDbType.Varchar2
                };
                param[2] = new OracleParameter(":ID_GRUPO", grupoFamilia.Id)
                {
                    OracleDbType = OracleDbType.Int32
                };

                qtdeRows = OracleHelper.ExecuteNonQuery(sql.ToString(), param);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return qtdeRows;
        }

        public int Excluir(int idGrupo)
        {
            int qtdeRows = 0;

            try
            {
                var sql = new StringBuilder();

                sql.AppendLine("UPDATE AMESP.GRUPO_FAMILIA ");
                sql.AppendLine(" SET    DT_EXCLUSAO       = :DT_EXCLUSAO");
                sql.AppendLine(" WHERE  ID_GRUPO          = :ID_GRUPO ");

                var param = new OracleParameter[2];

                param[0] = new OracleParameter(":DT_EXCLUSAO", System.DateTime.Now)
                {
                    OracleDbType = OracleDbType.Date
                };
                param[1] = new OracleParameter(":ID_GRUPO", idGrupo)
                {
                    OracleDbType = OracleDbType.Int32
                };

                qtdeRows = OracleHelper.ExecuteNonQuery(sql.ToString(), param);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return qtdeRows;
        }

        public tpGrupoFamilia Obter(int grupoId)
        {
            tpGrupoFamilia entidade = null;
            var sql = new StringBuilder();

            try
            {
                sql.Append(@"SELECT ID_GRUPO, ID_UNIDADE, NM_GRUPO, DT_CRIACAO, DT_EXCLUSAO FROM AMESP.GRUPO_FAMILIA");
                sql.AppendFormat(" WHERE ID_GRUPO = {0} ", grupoId);

                var dr = OracleHelper.ExecuteReader(sql.ToString());

                if (dr.HasRows)
                {
                    entidade = new tpGrupoFamilia();
                    while (dr.Read())
                    {
                        entidade.Id = dr["ID_GRUPO"].VerificaNuloInt();
                        entidade.CodigoUnidade = dr["ID_UNIDADE"].VerificaNuloString();
                        entidade.Nome = dr["NM_GRUPO"].VerificaNuloString();
                        entidade.Criacao = dr["DT_CRIACAO"].VerificaNuloDateTime();
                        entidade.Exclusao = dr["DT_EXCLUSAO"].VerificaNuloDateTime();
                    }
                }

                dr.Close();

                return entidade;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool VerificaTravaRegistro(int grupoId)
        {
            var sql = new StringBuilder();
            bool travado = false;

            try
            {
                sql.AppendLine("SELECT * FROM AMESP.GCP_CARTEIRA_PROFISSIONAL ");
                sql.AppendLine(" WHERE  ID_GRUPO = :ID_GRUPO ");

                var param = new OracleParameter[1];

                param[0] = new OracleParameter(":ID_GRUPO", grupoId)
                {
                    OracleDbType = OracleDbType.Int32
                };

                var dr = OracleHelper.ExecuteReader(sql.ToString(), param);

                if (dr.HasRows)
                {
                    travado = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return travado;
        }
    }
}