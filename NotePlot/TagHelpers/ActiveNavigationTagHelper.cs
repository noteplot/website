using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NotePlot.TagHelpers
{

    [HtmlTargetElement("li", Attributes = _for)]
    public class ActiveNavigationTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _UrlHelper;
        private const string _for = "navigation-active-for";

        public string NavigationActiveFor { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public ActiveNavigationTagHelper(IUrlHelperFactory urlHelper)
        {
            _UrlHelper = urlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _UrlHelper.GetUrlHelper(ViewContext);
            var currentPage = urlHelper.Action();
            if (NavigationActiveFor.EndsWith("*"))
            {
                if (currentPage.StartsWith(NavigationActiveFor.TrimEnd('*'), StringComparison.InvariantCultureIgnoreCase))
                    output.Attributes.Add("class", "active");
            }
            else
            {
                if (currentPage.Equals(NavigationActiveFor, StringComparison.InvariantCultureIgnoreCase))
                    output.Attributes.Add("class", "active");
            }
        }
    }
}
