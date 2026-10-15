using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models;

namespace WebApp.Services
{
    /// <summary>
    /// Service for loading and managing menus from the database
    /// Implements caching for performance optimization
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly SsgtContext _context;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY_ALL_MENUS = "all_menus";
        private const string CACHE_KEY_MENUS_BY_LEVEL = "menus_by_level_{0}";
        private const int CACHE_DURATION_MINUTES = 1;
        private const string CACHE_KEY_VERSION = "menu_cache_version";

        public MenuService(SsgtContext context, IMemoryCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Get all menus for a specific user level with hierarchical structure
        /// </summary>
        public async Task<List<MenuDTO>> GetMenusByUserLevelAsync(int userLevel)
        {
            string cacheKey = string.Format(CACHE_KEY_MENUS_BY_LEVEL, userLevel);

            if (_cache.TryGetValue(cacheKey, out List<MenuDTO>? cachedMenus))
            {
                return cachedMenus ?? new List<MenuDTO>();
            }

            // Get menus for this user level from database
            var menus = await _context.FwMenus
                .Where(m => m.UserLevel == userLevel && m.IsActive == true)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            // Build hierarchical structure
            var menuDtos = menus.Select(MenuDTO.FromEntity).ToList();
            var hierarchicalMenus = BuildHierarchy(menuDtos);

            // Cache the result
            _cache.Set(cacheKey, hierarchicalMenus, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return hierarchicalMenus;
        }

        /// <summary>
        /// Get all menus without filtering (for admin purposes)
        /// </summary>
        public async Task<List<MenuDTO>> GetAllMenusAsync()
        {
            if (_cache.TryGetValue(CACHE_KEY_ALL_MENUS, out List<MenuDTO>? cachedMenus))
            {
                return cachedMenus ?? new List<MenuDTO>();
            }

            // Get all menus from database
            var menus = await _context.FwMenus
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            // Build hierarchical structure
            var menuDtos = menus.Select(MenuDTO.FromEntity).ToList();
            var hierarchicalMenus = BuildHierarchy(menuDtos);

            // Cache the result
            _cache.Set(CACHE_KEY_ALL_MENUS, hierarchicalMenus, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return hierarchicalMenus;
        }

        /// <summary>
        /// Get a specific menu by ID
        /// </summary>
        public async Task<MenuDTO?> GetMenuByIdAsync(int menuId)
        {
            var menu = await _context.FwMenus.FirstOrDefaultAsync(m => m.MenuId == menuId);
            return menu != null ? MenuDTO.FromEntity(menu) : null;
        }

        /// <summary>
        /// Get all child menus for a parent menu
        /// </summary>
        public async Task<List<MenuDTO>> GetChildMenusAsync(int parentId)
        {
            var children = await _context.FwMenus
                .Where(m => m.ParentId == parentId && m.IsActive == true)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            return children.Select(MenuDTO.FromEntity).ToList();
        }

        /// <summary>
        /// Get root-level menus (ParentId = 0) for a user level
        /// </summary>
        public async Task<List<MenuDTO>> GetRootMenusAsync(int userLevel)
        {
            var rootMenus = await _context.FwMenus
                .Where(m => m.ParentId == 0 && m.UserLevel == userLevel && m.IsActive == true)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            var menuDtos = rootMenus.Select(MenuDTO.FromEntity).ToList();

            // Load children for each root menu
            foreach (var menu in menuDtos)
            {
                var children = await GetChildMenusAsync(menu.MenuId);
                if (children.Any())
                {
                    menu.Children = children;
                }
            }

            return menuDtos;
        }

        /// <summary>
        /// Invalidate the menu cache
        /// </summary>
        public void InvalidateCache()
        {
            // Remove all menu-related cache entries
            _cache.Remove(CACHE_KEY_ALL_MENUS);
            _cache.Remove(CACHE_KEY_VERSION);

            // Remove all user-level specific cache entries
            // This is a simplified approach; in production, you might track all keys
            for (int i = 0; i < 100; i++)
            {
                _cache.Remove(string.Format(CACHE_KEY_MENUS_BY_LEVEL, i));
            }
        }

        /// <summary>
        /// Check if user has access to a specific menu
        /// </summary>
        public async Task<bool> UserHasAccessAsync(int menuId, int userLevel)
        {
            var menu = await _context.FwMenus.FirstOrDefaultAsync(
                m => m.MenuId == menuId && m.UserLevel == userLevel && m.IsActive == true);

            return menu != null;
        }

        /// <summary>
        /// Build hierarchical menu structure from flat list
        /// </summary>
        private List<MenuDTO> BuildHierarchy(List<MenuDTO> menus)
        {
            var hierarchy = new List<MenuDTO>();

            // Get root menus (ParentId = 0)
            var rootMenus = menus.Where(m => m.ParentId == 0).OrderBy(m => m.SortOrder).ToList();

            foreach (var rootMenu in rootMenus)
            {
                rootMenu.Children = BuildChildrenRecursive(rootMenu.MenuId, menus);
                hierarchy.Add(rootMenu);
            }

            return hierarchy;
        }

        /// <summary>
        /// Recursively build children for a parent menu
        /// </summary>
        private List<MenuDTO>? BuildChildrenRecursive(int parentId, List<MenuDTO> allMenus)
        {
            var children = allMenus
                .Where(m => m.ParentId == parentId && m.ParentId != 0)
                .OrderBy(m => m.SortOrder)
                .ToList();

            if (!children.Any())
            {
                return null;
            }

            foreach (var child in children)
            {
                child.Children = BuildChildrenRecursive(child.MenuId, allMenus);
            }

            return children;
        }

        /// <summary>
        /// Get cache version to detect if menus have been updated
        /// </summary>
        public string GetCacheVersion()
        {
            if (!_cache.TryGetValue(CACHE_KEY_VERSION, out string? version))
            {
                version = Guid.NewGuid().ToString();
                _cache.Set(CACHE_KEY_VERSION, version, TimeSpan.FromHours(24));
            }
            return version;
        }

    }
}
