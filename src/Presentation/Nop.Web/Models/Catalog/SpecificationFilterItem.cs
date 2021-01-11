using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Represents a specification filter item
    /// </summary>
    public partial class SpecificationFilterItem : BaseNopModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the specification attribute name
        /// </summary>
        public string SpecificationAttributeName { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option name
        /// </summary>
        public string SpecificationAttributeOptionName { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option id
        /// </summary>
        public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option color (RGB)
        /// </summary>
        public string SpecificationAttributeOptionColorRgb { get; set; }

        /// <summary>
        /// Gets or sets the filter URL
        /// </summary>
        public string FilterUrl { get; set; }

        #endregion
    }
}
