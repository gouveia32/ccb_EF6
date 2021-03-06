﻿using Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

namespace Repository
{
    public class RepositoryPessoa : RepositoryBase<Pessoa>
    {
        public RepositoryPessoa() : base()
        {

        }

        public static IEnumerable<Pessoa> ObterTodos()
        {
            var pessoa = new List<Pessoa>();
            var sql = "SELECT * FROM Pessoas p " +
                "LEFT JOIN PessoasFisicas f ON f.Id = p.Id " +
                "LEFT JOIN PessoasJuridicas j ON j.Id = p.Id ";
            using (var context = new LoyaltyDB())
            {
                var con = context.Database.Connection;

                con.Query<Pessoa, PessoaFisica, PessoaJuridica, Pessoa>(sql, (p, f, j) =>   
                {   
                    if (p != null && !pessoa.Exists(src => src.Id == p.Id))
                    {
                        pessoa.Add(p);
                    }
                    if (pessoa.Count() > 0)
                    {
                        for (int i = 0; i < pessoa.Count(); i++)
                        {
                            if (f != null && pessoa[i].Id == f.Id)
                            {
                                pessoa[i].PessoaFisica = f;
                            }
                                if (j != null && pessoa[i].Id == j.Id)
                                {
                                    pessoa[i].PessoaJuridica = j;
                                }
                            }
                        }
                        return pessoa.FirstOrDefault();
                    });
                }
            return pessoa;
        }

        public Pessoa ObterPorIdEF(Guid id)
        {
            return dbSet.Find(id);
        }

        public static Pessoa ObterPorId(Guid id)
        {
            var pessoa = new List<Pessoa>();
            var sql = "SELECT * FROM Pessoas p " +
                "LEFT JOIN PessoasFisicas f ON f.Id = p.Id " +
                "LEFT JOIN PessoasFisica_Endereco x ON x.PessoasFisicaId = f.Id " +
                "LEFT JOIN Enderecos e ON e.Id = p.Id " +
                "WHERE p.Id = @sid;" +
                "SELECT * FROM Pessoas p " +
                "LEFT JOIN PessoasJuridicas j ON j.Id = p.Id " +
                "LEFT JOIN PessoasJuridica_Endereco y ON y.PessoasJuridicaId = j.Id " +
                "LEFT JOIN Enderecos e ON e.Id = p.Id " +
                "WHERE p.Id = @sid";
            using (var context = new LoyaltyDB())
            {
                var con = context.Database.Connection;
                using (var multi = con.QueryMultiple(sql, new { sid = id }))
                {
                    var pessoaFisica = multi.Read<Pessoa, PessoaFisica, Endereco, Pessoa>((p, f, e) =>
                    {
                        if (p != null && !pessoa.Exists(src => src.Id == p.Id))
                        {
                            pessoa.Add(p);
                            pessoa[0].PessoaFisica = f;
                        }
                        if (e != null)
                        {
                            pessoa[0].Endereco = e;
                        }
                        return pessoa.FirstOrDefault();
                    });

                    var pessoaJuridica = multi.Read<Pessoa, PessoaJuridica, Endereco, Pessoa>((p, j, e) =>
                    {
                        if (p != null && !pessoa.Exists(src => src.Id == p.Id))
                        {
                            pessoa.Add(p);

                        }
                        if (pessoa.Count() > 0)
                        {
                            for (int i = 0; i < pessoa.Count(); i++)
                            {
                                if (j != null && pessoa[i].Id == j.Id)
                                {
                                    pessoa[i].PessoaJuridica = j;
                                }
                                if (e != null)
                                {
                                    pessoa[0].Endereco = e;
                                }
                            }
                        }
                        return pessoa.FirstOrDefault();
                    });
                }
            }

            //con.Query<Pessoa, PessoaFisica, Endereco, Pessoa>(sql, (p, f, e) =>
            //{
            //    if (p != null && !pessoa.Exists(src => src.Id == p.Id))
            //    {
            //        pessoa.Add(p);
            //    }
            //    if (pessoa.Count() > 0)
            //    {
            //        for (int i = 0; i < pessoa.Count(); i++)
            //        {
            //            if (f != null && pessoa[i].Id == f.Id)
            //            {
            //                pessoa[i].PessoaFisica = f;
            //            }
            //            if (e != null)
            //            {
            //                pessoa[i].PessoaFisica.Endereco.Add(e);
            //            }
            //        }
            //    }
            //    return pessoa.FirstOrDefault();
            //});

            return pessoa.FirstOrDefault();

        }
    }
}