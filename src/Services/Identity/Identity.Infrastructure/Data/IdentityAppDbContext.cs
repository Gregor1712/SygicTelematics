using Identity.Application.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data;

public class IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
    : IdentityDbContext<AppUser>(options)
{
}