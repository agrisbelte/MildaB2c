namespace Milda.B2C.Function.Models;

public class AzureAdB2COptions
{
    public string Instance { get; set; }
    public string Domain { get; set; }
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string Audience { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string[] ValidIssuers { get; set; }
    public string[] ValidAudiences { get; set; }
}