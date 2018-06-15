using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NotePlot.TagHelpers
{

    [HtmlTargetElement("li", Attributes = attributes)]
    public class ActiveNavigationTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _UrlHelper;
        private const string attributes = "controller-name,action-name";

        public string ControllerName { get; set; }
        public string ActionName { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public ActiveNavigationTagHelper(IUrlHelperFactory urlHelper)
        {
            _UrlHelper = urlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _UrlHelper.GetUrlHelper(ViewContext);
            var currentControllerName = ViewContext.RouteData.Values["controller"].ToString();
            var currentActionName = ViewContext.RouteData.Values["action"].ToString();

            Char delimiter = ',';
            String[] СontrollerNames = ControllerName.Split(delimiter);
            foreach (var controllerName in СontrollerNames)
            {
                if (String.Equals(controllerName.Trim(), currentControllerName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ActionName.Equals("*", StringComparison.InvariantCultureIgnoreCase))
                    {
                        output.Attributes.Add("class", "active");
                    }
                    else if (ActionName.EndsWith("*"))
                    {
                        if (currentActionName.StartsWith(ActionName.TrimEnd('*'), StringComparison.InvariantCultureIgnoreCase))
                        {
                            output.Attributes.Add("class", "active");
                        }
                    }
                    else if (ActionName.Equals(currentActionName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        output.Attributes.Add("class", "active");
                    }
                }
            }
        }
    }
}
