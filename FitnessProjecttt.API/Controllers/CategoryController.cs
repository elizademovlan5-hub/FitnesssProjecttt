using FitnessProjecttt.Application.DTOs.Category;
using FitnessProjecttt.Domain.Entities;
using FitnessProjecttt.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories
            .Select(x => new
            {
                x.Name,
                x.Slug
            })
            .ToListAsync();

        return Ok(categories);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCategoryDto dto)
    {
        var slug = dto.Name
            .Trim()
            .ToLower()
            .Replace(" ", "-");

        var category = new Category
        {
            Name = dto.Name,
            Slug = slug
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{slug}")]
    public async Task<IActionResult> Update(
        string slug,
        UpdateCategoryDto dto)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Slug == slug);

        if (category == null)
            return NotFound("Category tapilmadi.");

        category.Name = dto.Name;
        category.Slug = dto.Name
            .Trim()
            .ToLower()
            .Replace(" ", "-");

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete(
        string slug)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Slug == slug);

        if (category == null)
            return NotFound("Category tapilmadi.");

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return Ok("Category silindi.");
    }
}