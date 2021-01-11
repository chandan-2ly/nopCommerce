using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Represents a specification filter model
    /// </summary>
    public partial class SpecificationFilterModel : BaseNopModel
    {
        #region Const

        private const string QUERYSTRINGPARAM = "specs";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the filter items
        /// </summary>
        public IList<SpecificationFilterItem> FilterItems { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public SpecificationFilterModel()
        {
            FilterItems = new List<SpecificationFilterItem>();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Exclude query string parameters
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>New URL</returns>
        protected virtual Task<string> ExcludeQueryStringParamsAsync(string url, IWebHelper webHelper)
        {
            //comma separated list of parameters to exclude
            const string excludedQueryStringParams = "pagenumber";
            var excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var exclude in excludedQueryStringParamsSplitted)
                url = webHelper.RemoveQueryString(url, exclude);

            return Task.FromResult(url);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get IDs of already filtered specification options
        /// </summary>
        /// <param name="webHelper">Web helper</param>
        /// <returns>IDs</returns>
        public virtual Task<List<int>> GetAlreadyFilteredSpecOptionIdsAsync(IWebHelper webHelper)
        {
            var result = new List<int>();

            var alreadyFilteredSpecsStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
            if (string.IsNullOrWhiteSpace(alreadyFilteredSpecsStr))
                return Task.FromResult(result);

            foreach (var spec in alreadyFilteredSpecsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int.TryParse(spec.Trim(), out var specId);
                if (!result.Contains(specId))
                    result.Add(specId);
            }
            return Task.FromResult(result);
        }

        // <summary>
        /// Prepare model
        /// </summary>
        /// <param name="alreadyFilteredSpecOptionIds">IDs of already filtered specification options</param>
        /// <param name="filterableSpecificationAttributeOptionIds">IDs of filterable specification options</param>
        /// <param name="specificationAttributeService"></param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="workContext">Work context</param>
        /// <param name="staticCacheManager">Cache manager</param>
        public virtual async Task PrepareSpecsFiltersAsync(IList<int> alreadyFilteredSpecOptionIds,
            int[] filterableSpecificationAttributeOptionIds,
                ISpecificationAttributeService specificationAttributeService, ILocalizationService localizationService,
            IWebHelper webHelper, IWorkContext workContext, IStaticCacheManager staticCacheManager)
        {
            Enabled = false;

            var cacheKey = staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SpecsFilterModelKey, filterableSpecificationAttributeOptionIds, await workContext.GetWorkingLanguageAsync());

            var allOptions = await specificationAttributeService.GetSpecificationAttributeOptionsByIdsAsync(filterableSpecificationAttributeOptionIds);
            var allFilters = await staticCacheManager.GetAsync(cacheKey, async () => await allOptions.SelectAwait(async sao =>
            {
                var specAttribute = await specificationAttributeService.GetSpecificationAttributeByIdAsync(sao.SpecificationAttributeId);

                return new SpecificationAttributeOptionFilter
                {
                    SpecificationAttributeId = specAttribute.Id,
                    SpecificationAttributeName = await localizationService.GetLocalizedAsync(specAttribute, x => x.Name, (await workContext.GetWorkingLanguageAsync()).Id),
                    SpecificationAttributeDisplayOrder = specAttribute.DisplayOrder,
                    SpecificationAttributeOptionId = sao.Id,
                    SpecificationAttributeOptionName = await localizationService.GetLocalizedAsync(sao, x => x.Name, (await workContext.GetWorkingLanguageAsync()).Id),
                    SpecificationAttributeOptionColorRgb = sao.ColorSquaresRgb,
                    SpecificationAttributeOptionDisplayOrder = sao.DisplayOrder
                };
            }).ToListAsync());

            if (!allFilters.Any())
                return;

            //sort loaded options
            allFilters = allFilters.OrderBy(saof => saof.SpecificationAttributeDisplayOrder)
                .ThenBy(saof => saof.SpecificationAttributeName)
                .ThenBy(saof => saof.SpecificationAttributeOptionDisplayOrder)
                .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();

            //prepare the model properties
            Enabled = true;

            //get not filtered specification options
            FilterItems = await allFilters.SelectAwait(async x =>
            {
                //filter URL
                var alreadyFiltered = alreadyFilteredSpecOptionIds.Concat(new List<int> { x.SpecificationAttributeOptionId });
                var filterUrl = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM,
                    alreadyFiltered.OrderBy(id => id).Select(id => id.ToString()).ToArray());

                return new SpecificationFilterItem
                {
                    SpecificationAttributeName = x.SpecificationAttributeName,
                    SpecificationAttributeOptionId = x.SpecificationAttributeOptionId,
                    SpecificationAttributeOptionName = x.SpecificationAttributeOptionName,
                    SpecificationAttributeOptionColorRgb = x.SpecificationAttributeOptionColorRgb,
                    FilterUrl = await ExcludeQueryStringParamsAsync(filterUrl, webHelper)
                };
            }).ToListAsync();
        }

        #endregion
    }
}
