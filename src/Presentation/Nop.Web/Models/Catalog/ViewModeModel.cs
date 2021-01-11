using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Represents a view mode model of the catalog products
    /// </summary>
    public class ViewModeModel : BaseNopModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the view mode is selected
        /// </summary>
        public bool Selected { get; set; }

        #endregion
    }
}
