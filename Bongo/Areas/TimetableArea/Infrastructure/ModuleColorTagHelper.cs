using Bongo.Areas.TimetableArea.Models;
using Bongo.Data;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bongo.Areas.TimetableArea.Infrastructure
{
    [HtmlTargetElement("td", Attributes = "module-color")]
    public class ModuleColorTagHelper : TagHelper
    {
        private readonly IEndpointWrapper wrapper;
        public ModuleColorTagHelper(IEndpointWrapper _wrapper)
        {
            wrapper = _wrapper;
        }


        [HtmlAttributeName("module-color")]
        public string ModuleCode { get; set; }

        [HtmlAttributeName("module-color-username")]
        public string Username { get; set; }

        public override async void Process(TagHelperContext context,
            TagHelperOutput output)
        {
            var response = await wrapper.Color.GetModuleColorWithColorDetails(ModuleCode);
            var module = await response.Content.ReadFromJsonAsync<ModuleColor>();
            if (module.Color.ColorName != "No-color")
            {
                output.Attributes.SetAttribute("style", $"background-color: {module.Color.ColorValue}; color: white");
            }
            else
            {
                output.Attributes.SetAttribute("style", "color: black");
            }
        }
    }
}
