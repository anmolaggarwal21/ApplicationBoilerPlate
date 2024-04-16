using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    public class VersionNeutralApiController : BaseApiController
    {
    }

}
