using System;

namespace MyDigimal.Data.Entities;

public class UserExternalAuthEntity
{
    public Guid UserId { get; set; }
    public string ProviderKey { get; set; }
    public DateTime LastLogin { get; set; }
}