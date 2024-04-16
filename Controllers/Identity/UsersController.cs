using Application.Identity.Users;
using DocumentFormat.OpenXml.Math;
using Infrastructure.Auth.Permissions;
using Infrastructure.Identity;
using Infrastructure.OpenAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Authorization;

namespace ClinicManagement.Controllers.Identity
{

    public class UsersController : VersionNeutralApiController
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }


        [HttpGet]
        [MustHavePermission(ClinicAction.View, ClinicResource.Users)]
        [OpenApiOperation("Get list of all users.", "")]
        public Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken)
        {
            return _userService.GetListAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        [MustHavePermission(ClinicAction.View, ClinicResource.Users)]
        [OpenApiOperation("Get a user's details.", "")]
        public Task<UserDetailsDto> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return _userService.GetAsync(id, cancellationToken);
        }

        [HttpGet("{id}/roles")]
        [MustHavePermission(ClinicAction.View, ClinicResource.UserRoles)]
        [OpenApiOperation("Get a user's roles.", "")]
        public Task<List<UserRoleDto>> GetRolesAsync(string id, CancellationToken cancellationToken)
        {
            return _userService.GetRolesAsync(id, cancellationToken);
        }

        [HttpPost("{id}/roles")]
        [ApiConventionMethod(typeof(ApplicationApiConventions), nameof(ApplicationApiConventions.Register))]
        [MustHavePermission(ClinicAction.Update, ClinicResource.UserRoles)]
        [OpenApiOperation("Update a user's assigned roles.", "")]
        public Task<string> AssignRolesAsync(string id, UserRolesRequest request, CancellationToken cancellationToken)
        {
            return _userService.AssignRolesAsync(id, request, cancellationToken);
        }

        [HttpPost]
        [MustHavePermission(ClinicAction.Create, ClinicResource.Users)]
        [OpenApiOperation("Creates a new user.", "")]
        public Task<string> CreateAsync(CreateUserRequest request)
        {
            // TODO: check if registering anonymous users is actually allowed (should probably be an appsetting)
            // and return UnAuthorized when it isn't
            // Also: add other protection to prevent automatic posting (captcha?)
            return _userService.CreateAsync(request, GetOriginFromRequest());
        }

        [HttpPost("self-register")]
        [TenantIdHeader]
        [AllowAnonymous]
        [OpenApiOperation("Anonymous user creates a user.", "")]
        [ApiConventionMethod(typeof(ApplicationApiConventions), nameof(ApplicationApiConventions.Register))]
        public Task<string> SelfRegisterAsync(CreateUserRequest request)
        {
            // TODO: check if registering anonymous users is actually allowed (should probably be an appsetting)
            // and return UnAuthorized when it isn't
            // Also: add other protection to prevent automatic posting (captcha?)
            return _userService.CreateAsync(request, GetOriginFromRequest());
        }

        [HttpPost("{id}/toggle-status")]
        [MustHavePermission(ClinicAction.Update, ClinicResource.Users)]
        [ApiConventionMethod(typeof(ApplicationApiConventions), nameof(ApplicationApiConventions.Register))]
        [OpenApiOperation("Toggle a user's active status.", "")]
        public async Task<ActionResult> ToggleStatusAsync(string id, ToggleUserStatusRequest request, CancellationToken cancellationToken)
        {
            if (id != request.UserId)
            {
                return BadRequest();
            }

            await _userService.ToggleStatusAsync(request, cancellationToken);
            return Ok();
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        [OpenApiOperation("Confirm email address for a user.", "")]
        [ApiConventionMethod(typeof(ApplicationApiConventions), nameof(ApplicationApiConventions.Search))]
        public Task<string> ConfirmEmailAsync([FromQuery] string tenant, [FromQuery] string userId, [FromQuery] string code, CancellationToken cancellationToken)
        {
            return _userService.ConfirmEmailAsync(userId, code, tenant, cancellationToken);
        }

        [HttpGet("confirm-phone-number")]
        [AllowAnonymous]
        [OpenApiOperation("Confirm phone number for a user.", "")]
        [ApiConventionMethod(typeof(ApplicationApiConventions), nameof(ApplicationApiConventions.Search))]
        public Task<string> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
        {
            return _userService.ConfirmPhoneNumberAsync(userId, code);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [TenantIdHeader]
        [OpenApiOperation("Request a password reset email for a user.", "")]
        [ApiConventionMethod(typeof(ApplicationApiConventions), nameof(ApplicationApiConventions.Register))]
        public Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            return _userService.ForgotPasswordAsync(request, GetOriginFromRequest());
        }

        [HttpPost("reset-password")]
        [OpenApiOperation("Reset a user's password.", "")]
        [ApiConventionMethod(typeof(ApplicationApiConventions), nameof(ApplicationApiConventions.Register))]
        public Task<string> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return _userService.ResetPasswordAsync(request);
        }

        private string GetOriginFromRequest() => $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
    }

}
