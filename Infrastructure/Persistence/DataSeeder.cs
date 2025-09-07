using System;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CompatibleAPI.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedData(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                if (await context.Profiles.AnyAsync())
                {
                    logger.LogInformation("Database already seeded");
                    return;
                }

                logger.LogInformation("Seeding database...");

                // Create test profiles with realistic data
                var profiles = new[]
                {
                    new Profile
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        Name = "John Doe",
                        Age = 28,
                        Bio = "Software engineer who loves hiking and photography",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Photos = new List<Photo> {
                            new Photo { Url = "https://randomuser.me/api/portraits/men/1.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                            new Photo { Url = "https://randomuser.me/api/portraits/men/11.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                        }
                    },
                    new Profile
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        Name = "Jane Smith",
                        Age = 26,
                        Bio = "Artist and yoga instructor. Passionate about creativity and mindfulness. Looking for someone to share adventures with.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Photos = new List<Photo> {
                            new Photo { Url = "https://randomuser.me/api/portraits/women/2.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                            new Photo { Url = "https://randomuser.me/api/portraits/women/12.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                        }
                    },
                    new Profile
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                        Name = "Mike Johnson",
                        Age = 30,
                        Bio = "Chef who enjoys traveling and trying new cuisines. Love cooking for others and exploring new restaurants.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Photos = new List<Photo> {
                            new Photo { Url = "https://randomuser.me/api/portraits/men/3.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                        }
                    },
                    new Profile
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                        Name = "Sarah Williams",
                        Age = 27,
                        Bio = "Doctor who loves reading and outdoor activities. Looking for someone to share adventures with.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Photos = new List<Photo> {
                            new Photo { Url = "https://randomuser.me/api/portraits/women/4.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                        }
                    },
                    new Profile
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                        Name = "Alex Chen",
                        Age = 29,
                        Bio = "Tech entrepreneur and fitness enthusiast. Love trying new things and meeting new people.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Photos = new List<Photo> {
                            new Photo { Url = "https://randomuser.me/api/portraits/men/5.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                        }
                    },
                    new Profile
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                        Name = "Emma Wilson",
                        Age = 25,
                        Bio = "Environmental scientist and nature lover. Looking for someone to share adventures with.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Photos = new List<Photo> {
                            new Photo { Url = "https://randomuser.me/api/portraits/women/6.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                        }
                    }
                };

                await context.Profiles.AddRangeAsync(profiles);
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeded successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
} 