﻿using Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Repository
{
    public class RepositoryBase<TEntity> where TEntity : class
    {
        protected CCB_Context context;
        protected DbSet<TEntity> dbSet;
        public RepositoryBase()
        {
            context = new CCB_Context();
            dbSet = context.Set<TEntity>();
            context.Database.Log = x => Debug.Write(x);
        }

        public virtual TEntity ObterPorId(Guid id)
        {
            var result = dbSet.Find(id);

            return result;
        }

        public IEnumerable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Where(predicate);
        }

        public IEnumerable<TEntity> BuscarComPropriedades(Expression<Func<TEntity, bool>> predicate, string includeProperties = null, string selected = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (selected != null)
                    {
                        query = query.Include(includeProperty).Include(includeProperty + "." + selected);
                    }
                    else
                    {
                        query = query.Include(includeProperty);
                    }

                }
            }

            return query.ToList();
        }

        public void Adicionar(TEntity tentity)
        {
            dbSet.Add(tentity);
            context.SaveChanges();
        }

        public TEntity Atualizar(TEntity tentity)
        {

            var entry = context.Entry(tentity);
            dbSet.Attach(tentity);
            entry.State = EntityState.Modified;
            context.SaveChanges();
            return tentity;
        }

        public void Remover(Guid id)
        {
            var result = dbSet.Find(id);
            dbSet.Remove(result);
            context.SaveChanges();
        }

        public void Remover(TEntity tentity)
        {
            //var entry = context.Entry(tentity);
            dbSet.Attach(tentity);
            dbSet.Remove(tentity);
            context.SaveChanges();
        }

        public IEnumerable<TEntity> ObterTodos(string includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query.ToList();
        }
    }
}