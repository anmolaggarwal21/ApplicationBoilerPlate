using Application.Identity.Roles;
using DocumentFormat.OpenXml.Math;
using Infrastructure.Auth.Permissions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Authorization;

namespace ClinicManagement.Controllers.Identity
{

    public class RolesController : VersionNeutralApiController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService) => _roleService = roleService;

        [HttpGet]
        [MustHavePermission(ClinicAction.View, ClinicResource.Roles)]
        [OpenApiOperation("Get a list of all roles.", "")]
        public Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken)
        {
            return _roleService.GetListAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        [MustHavePermission(ClinicAction.View, ClinicResource.Roles)]
        [OpenApiOperation("Get role details.", "")]
        public Task<RoleDto> GetByIdAsync(string id)
        {
            return _roleService.GetByIdAsync(id);
        }

        [HttpGet("{id}/permissions")]
        [MustHavePermission(ClinicAction.View, ClinicResource.RoleClaims)]
        [OpenApiOperation("Get role details with its permissions.", "")]
        public Task<RoleDto> GetByIdWithPermissionsAsync(string id, CancellationToken cancellationToken)
        {
            return _roleService.GetByIdWithPermissionsAsync(id, cancellationToken);
        }

        [HttpPut("{id}/permissions")]
        [MustHavePermission(ClinicAction.Update, ClinicResource.RoleClaims)]
        [OpenApiOperation("Update a role's permissions.", "")]
        public async Task<ActionResult<string>> UpdatePermissionsAsync(string id, UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
        {
            if (id != request.RoleId)
            {
                return BadRequest();
            }

            return Ok(await _roleService.UpdatePermissionsAsync(request, cancellationToken));
        }

        [HttpPost]
        [MustHavePermission(ClinicAction.Create, ClinicResource.Roles)]
        [OpenApiOperation("Create or update a role.", "")]
        public Task<string> RegisterRoleAsync(CreateOrUpdateRoleRequest request)
        {
            return _roleService.CreateOrUpdateAsync(request);
        }

        [HttpDelete("{id}")]
        [MustHavePermission(ClinicAction.Delete, ClinicResource.Roles)]
        [OpenApiOperation("Delete a role.", "")]
        public Task<string> DeleteAsync(string id)
        {
            return _roleService.DeleteAsync(id);
        }
    }
}
