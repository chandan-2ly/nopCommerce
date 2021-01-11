using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Represents a products displaying model
    /// </summary>
    public partial class CatalogProductsDisplayingModel : BaseNopModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether product sorting is allowed
        /// </summary>
        public bool AllowProductSorting { get; set; }

        /// <summary>
        /// Gets or sets available sort options
        /// </summary>
        public IList<SortOptionModel> AvailableSortOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change view mode
        /// </summary>
        public bool AllowProductViewModeChanging { get; set; }

        /// <summary>
        /// Gets or sets available view mode options
        /// </summary>
        public IList<ViewModeModel> AvailableViewModes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets available page size options
        /// </summary>
        public IList<PageSizeModel> PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a order by
        /// </summary>
        public int? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a product sorting
        /// </summary>
        public string ViewMode { get; set; }

        #endregion

        #region Ctor

        public CatalogProductsDisplayingModel()
        {
            AvailableSortOptions = new List<SortOptionModel>();
            AvailableViewModes = new List<ViewModeModel>();
            PageSizeOptions = new List<PageSizeModel>();
        }

        #endregion
    }
}
