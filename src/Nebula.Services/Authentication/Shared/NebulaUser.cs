using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Shared
{
    public class NebulaUser : ClaimsPrincipal
    {
        // Global Roles
        public const string ROLE_GLOBAL_ADMIN = "global_admin";
        public const string ROLE_GLOBAL_SYSTEM_AUDITOR = "global_system_auditor";
        public const string ROLE_GLOBAL_SUPPORT_AGENT = "global_support_agent";

        // Org Roles (Core)
        public const string ROLE_ORG_OWNER = "org_owner";
        public const string ROLE_ORG_ADMIN = "org_admin";
        public const string ROLE_ORG_MANAGER = "org_manager";
        public const string ROLE_ORG_USER = "org_user";
        public const string ROLE_ORG_VIEWER = "org_viewer";

        // Org Roles (Extended)
        public const string ROLE_ORG_INVENTORY_MANAGER = "org_inventory_manager";
        public const string ROLE_ORG_INVENTORY_AUDITOR = "org_inventory_auditor";
        public const string ROLE_ORG_HR_MANAGER = "org_hr_manager";
        public const string ROLE_ORG_BILLING_MANAGER = "org_billing_manager";
        public const string ROLE_ORG_OPERATIONS_LEAD = "org_operations_lead";
        public const string ROLE_ORG_INTEGRATIONS_ADMIN = "org_integrations_admin";
        public const string ROLE_ORG_ACCOUNTING_MANAGER = "org_accounting_manager";
        public const string ROLE_ORG_ACCOUNTANT = "org_accountant";
        public const string ROLE_ORG_FINANCIAL_AUDITOR = "org_financial_auditor";



        // Claim Type Constants
        public const string IdType = "Id";
        public const string UserNameType = "sub";
        public const string DisplayNameType = "name";
        public const string FirstNameType = "givenname";
        public const string LastNameType = "surname";
        public const string IdentsType = "Idents";
        public const string RolesType = ClaimTypes.Role;

        // Properties
        public Guid Id { get; set; } = Guid.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<string> Idents { get; private set; } = new();
        public List<string> Roles { get; private set; } = new(); // Global roles only
        public Dictionary<Guid, List<string>> OrgRoles { get; private set; } = new(); // Scoped org roles
        public string JwtToken { get; set; } = string.Empty;
        public List<Claim> ExtraClaims { get; private set; } = new();
        public bool IsLoggedIn => Id != Guid.Empty;

        // Global Role Checks
        public bool IsGlobalAdmin => IsInRole(ROLE_GLOBAL_ADMIN);
        public bool IsGlobalSystemAuditor => IsInRole(ROLE_GLOBAL_SYSTEM_AUDITOR);
        public bool IsGlobalSupportAgent => IsInRole(ROLE_GLOBAL_SUPPORT_AGENT);

        // Scoped Org Role Check
        public bool IsInRoleForOrg(Guid orgId, string role)
        {
            return OrgRoles.TryGetValue(orgId, out var roles) && roles.Contains(role);
        }

        public bool IsOrgAdmin(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_ADMIN);
        public bool IsOrgOwner(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_OWNER);
        public bool IsOrgManager(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_MANAGER);
        public bool IsOrgUser(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_USER);
        public bool IsOrgViewer(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_VIEWER);
        public bool IsOrgAdminOrHigher(Guid orgId) => IsOrgOwner(orgId) || IsOrgAdmin(orgId) || IsGlobalAdmin;
        public bool IsOrgManagerOrHigher(Guid orgId) => IsOrgAdminOrHigher(orgId) || IsOrgManager(orgId);
        public bool IsInventoryManager(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_INVENTORY_MANAGER);
        public bool IsInventoryAuditor(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_INVENTORY_AUDITOR);
        public bool IsHRManager(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_HR_MANAGER);
        public bool IsBillingManager(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_BILLING_MANAGER);
        public bool IsOperationsLead(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_OPERATIONS_LEAD);
        public bool IsIntegrationsAdmin(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_INTEGRATIONS_ADMIN);
        public bool IsAccountingManager(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_ACCOUNTING_MANAGER);
        public bool IsAccountant(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_ACCOUNTANT);
        public bool IsFinancialAuditor(Guid orgId) => IsInRoleForOrg(orgId, ROLE_ORG_FINANCIAL_AUDITOR);

        // Optional hierarchy helpers
        public bool IsAccountingManagerOrHigher(Guid orgId) =>
            IsOrgManagerOrHigher(orgId) || IsAccountingManager(orgId);

        public bool IsAccountantOrHigher(Guid orgId) =>
            IsAccountingManagerOrHigher(orgId) || IsAccountant(orgId);


        // Optional Role Hierarchy Helpers
        public bool IsInventoryManagerOrHigher(Guid orgId) =>
            IsOrgManagerOrHigher(orgId) || IsInventoryManager(orgId);

        public bool IsBillingManagerOrHigher(Guid orgId) =>
            IsOrgManagerOrHigher(orgId) || IsBillingManager(orgId);


        // Claims
        public IEnumerable<Claim> ToClaims()
        {
            if (Id != Guid.Empty)
                yield return new Claim(IdType, Id.ToString());

            if (Idents.Count != 0)
                yield return new Claim(IdentsType, string.Join(';', Idents));

            if (!string.IsNullOrWhiteSpace(UserName))
                yield return new Claim(UserNameType, UserName);

            if (!string.IsNullOrWhiteSpace(DisplayName))
                yield return new Claim(DisplayNameType, DisplayName);

            if (!string.IsNullOrWhiteSpace(FirstName))
                yield return new Claim(FirstNameType, FirstName);

            if (!string.IsNullOrWhiteSpace(LastName))
                yield return new Claim(LastNameType, LastName);

            foreach (var r in Roles)
                yield return new Claim(RolesType, r);

            foreach (var c in ExtraClaims)
                yield return c;
        }

        public override bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }

        public static NebulaUser Parse(Claim[] claims)
        {
            Console.WriteLine($"[NebulaUser.Parse] Parsing {claims?.Length ?? 0} claims");
            if (claims == null || claims.Length == 0)
            {
                Console.WriteLine("[NebulaUser.Parse] No claims provided");
                return null;
            }

            var user = new NebulaUser();
            foreach (var claim in claims)
            {
                Console.WriteLine($"[NebulaUser.Parse] Processing claim: {claim.Type} = {claim.Value}");
                user.LoadClaim(claim);
            }

            Console.WriteLine($"[NebulaUser.Parse] User validation - Id: {user.Id}, Idents: {user.Idents.Count}, Roles: {user.Roles.Count}, OrgRoles: {user.OrgRoles.Count}");
            Console.WriteLine($"[NebulaUser.Parse] FirstName: '{user.FirstName}', LastName: '{user.LastName}'");
            Console.WriteLine($"[NebulaUser.Parse] IsValid: {user.IsValid}");

            return user.IsValid ? user : null;
        }

        private bool IsValid =>
            Id != Guid.Empty && Idents.Count > 0 && (Roles.Count > 0 || OrgRoles.Count > 0);

        private void LoadClaim(Claim claim)
        {
            Console.WriteLine($"FOUND CLAIM {claim.ToString()}");
            switch (claim.Type)
            {
                case IdType:
                    if (Guid.TryParse(claim.Value, out var id))
                        Id = id;
                    break;
                case UserNameType:
                    UserName = claim.Value;
                    break;
                case DisplayNameType:
                    DisplayName = claim.Value;
                    break;
                case FirstNameType:
                    Console.WriteLine($"[LoadClaim] Setting FirstName to: {claim.Value}");
                    FirstName = claim.Value;
                    break;
                case LastNameType:
                    Console.WriteLine($"[LoadClaim] Setting LastName to: {claim.Value}");
                    LastName = claim.Value;
                    break;
                case IdentsType:
                    Idents = claim.Value.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                    break;
                case RolesType:
                    Roles.Add(claim.Value); // Still used for global roles
                    break;
                default:
                    ExtraClaims.Add(claim);
                    break;
            }
        }

        // Load org roles from proto (you can call this manually after deserialization)
        public void LoadOrgRoles(IEnumerable<(Guid OrgId, IEnumerable<string> Roles)> orgRoleAssignments)
        {
            foreach (var (orgId, roles) in orgRoleAssignments)
            {
                OrgRoles[orgId] = roles.ToList();
            }
        }
    }
}
