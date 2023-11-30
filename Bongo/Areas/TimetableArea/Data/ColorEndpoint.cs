namespace Bongo.Areas.TimetableArea.Data
{
    public class ColorEndpoint : TimetableAreaBaseEndpoint, IColorEndpoint
    {
        public ColorEndpoint(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            URI += "Color";
        }

        /// <summary>
        /// Gets the list of all colors
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HttpResponseMessage> GetAllColors()
        {
            return await Client.GetAsync(new Uri($"{URI}/GetAllColors"));
        }

        /// <summary>
        /// Gets the specified module with it's color details
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetModuleColorWithColorDetails(string moduleCode)
        {
            return await Client.GetAsync(new Uri($"{URI}/GetModuleColorWithColorDetails/{moduleCode}"));
        }

        /// <summary>
        /// Gets the modules with colors for management.
        /// </summary>
        /// <returns>StatusCode 200 with a ModulesColorsViewModel object containing lists of ModuleColor and Color objects</returns>
        public async Task<HttpResponseMessage> GetModulesWithColors()
        {
            return await Client.GetAsync(new Uri($"{URI}/GetModulesWithColors"));
        }
    }
}
