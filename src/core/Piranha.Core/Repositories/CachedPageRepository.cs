/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Piranha.Models;

namespace Piranha.Repositories
{
    internal class CachedPageRepository : IPageRepository
    {
        #region Members
        private readonly IDistributedCache cache;
        private readonly IPageRepository repository;
        private readonly ILogger logger;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repository">The data service repository</param>
        /// <param name="cache">The configured cache</param>
        public CachedPageRepository(IPageRepository repository, IDistributedCache cache, ILoggerFactory logFactory) {
            this.cache = cache;
            this.repository = repository;
            this.logger = logFactory.CreateLogger("Piranha.Repositories.CachedPageRepository");
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <returns>The page model</returns>
        public PageModel GetStartpage() {
            PageModel model = null;

            if (cache != null)
                model = cache.GetModel<PageModel>(Guid.Empty.ToString());

            if (model == null) {
                logger.LogInformation("Dynamic startpage not found in cache, calling data service.");

                model = repository.GetStartpage();
                if (model != null && cache != null)
                    cache.SetModel(Guid.Empty.ToString(), model);
            } else {
                logger.LogInformation("Dynamic startpage found in cache.");                
            }
            return model;
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The page model</returns>
        public T GetStartpage<T>() where T : PageModel<T> {
            T model = null;

            if (cache != null)
                model = cache.GetModel<T>(Guid.Empty.ToString());

            if (model == null) {
                logger.LogInformation("Typed startpage not found in cache, calling data service.");

                model = repository.GetStartpage<T>();
                if (model != null && cache != null)
                    cache.SetModel(Guid.Empty.ToString(), model);
            } else {
                logger.LogInformation("Typed startpage found in cache.");
            }
            return model;
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public PageModel GetById(Guid id) {
            PageModel model = null;

            if (cache != null)
                model = cache.GetModel<PageModel>(id.ToString());

            if (model == null) {
                logger.LogInformation($"Dynamic page '{id}' not found in cache.");

                model = repository.GetById(id);
                if (model != null && cache != null)
                    cache.SetModel(id.ToString(), model);
            } else {
                logger.LogInformation($"Dynamic page '{id}' found in cache.");                                
            }
            return model;
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public  T GetById<T>(Guid id) where T : PageModel<T> {
            T model = null;

            if (cache != null)
                model = cache.GetModel<T>(id.ToString());

            if (model == null) {
                logger.LogInformation($"Typed page '{id}' not found in cache.");

                model = repository.GetById<T>(id);
                if (model != null && cache != null)
                    cache.SetModel(id.ToString(), model);
            } else {
                logger.LogInformation($"Typed page '{id}' found in cache.");                
            }
            return model;
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The page model</returns>
        public PageModel GetBySlug(string slug) {
            PageModel model = null;

            if (cache != null) {
                var pageId = cache.GetModel<Guid?>("PageSlug" + slug);
                if (pageId.HasValue)
                    model = cache.GetModel<PageModel>(pageId.Value.ToString());
            }

            if (model == null) {
                model = repository.GetBySlug(slug);
                if (model != null && cache != null) {
                    cache.SetModel("PageSlug" + model.Slug, model.Id);
                    cache.SetModel(model.Id.ToString(), model);
                }
            }
            return model;
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <returns>The page model</returns>
        public T GetBySlug<T>(string slug) where T : PageModel<T> {
            T model = null;

            if (cache != null) {
                var pageId = cache.GetModel<Guid?>("PageSlug" + slug);
                if (pageId.HasValue)
                    model = cache.GetModel<T>(pageId.Value.ToString());
            }

            if (model == null) {
                model = repository.GetBySlug<T>(slug);
                if (model != null && cache != null) {
                    cache.SetModel("PageSlug" + model.Slug, model.Id);
                    cache.SetModel(model.Id.ToString(), model);
                }
            }
            return model;
        }

        /// <summary>
        /// Gets all page models with the specified parent id.
        /// </summary>
        /// <param name="parentId">The parent id</param>
        /// <returns>The page models</returns>
        public IList<PageModel> GetByParentId(Guid? parentId) {
            throw new NotImplementedException();            
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        public void Save<T>(T model) where T : PageModel<T> {
            repository.Save(model);
        }

        /// <summary>
        /// Deletes the given page.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to delete</param>
        public void Delete<T>(T model) where T : PageModel<T> {
            repository.Delete(model);
        }

        /// <summary>
        /// Delets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id) {
            repository.Delete(id);
        }       
    }
}