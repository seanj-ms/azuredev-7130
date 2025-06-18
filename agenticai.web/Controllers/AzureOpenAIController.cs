using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using agenticai.app;

namespace agenticai.web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AzureOpenAIController : ControllerBase
    {
        private readonly agenticai.app.ConnectWithAzureOpenAI _openai;

        public AzureOpenAIController(agenticai.app.ConnectWithAzureOpenAI openai)
        {
            _openai = openai;
        }

        /// <summary>
        /// Invokes the Azure OpenAI RunAsync method with an input string.
        /// </summary>
        /// <param name="input">The input prompt string.</param>
        /// <returns>The response from Azure OpenAI.</returns>
        [HttpPost("run")]
        public async Task<IActionResult> RunAsync([FromBody] InputRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Input))
                return BadRequest("Input cannot be empty.");

            var result = await _openai.RunAsync(request.Input);
            return Ok(new { result });
        }
    }

    public class InputRequest
    {
        public string Input { get; set; }
    }
}