using WebApp.Models;

namespace WebApp.Services
{
    /// <summary>
    /// Interface for menu management and retrieval from database
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// Get all menus for a specific user role/level with hierarchical structure
        /// </summary>
        /// <param name="userLevel">User's role level (from FwUserDetail.RoleId or User claims)</param>
        /// <returns>List of root-level menus with child menus nested</returns>
        Task<List<MenuDTO>> GetMenusByUserLevelAsync(int userLevel);

        /// <summary>
        /// Get all menus without filtering (for admin purposes)
        /// </summary>
        /// <returns>All menus organized hierarchically</returns>
        Task<List<MenuDTO>> GetAllMenusAsync();

        /// <summary>
        /// Get a specific menu by ID
        /// </summary>
        /// <param name="menuId">Menu ID</param>
        /// <returns>MenuDTO or null if not found</returns>
        Task<MenuDTO?> GetMenuByIdAsync(int menuId);

        /// <summary>
        /// Get all child menus for a parent menu
        /// </summary>
        /// <param name="parentId">Parent menu ID</param>
        /// <returns>List of child menus</returns>
        Task<List<MenuDTO>> GetChildMenusAsync(int parentId);

        /// <summary>
        /// Get root-level menus (ParentId = 0) for a user level
        /// </summary>
        /// <param name="userLevel">User's role level</param>
        /// <returns>List of root menus</returns>
        Task<List<MenuDTO>> GetRootMenusAsync(int userLevel);

        /// <summary>
        /// Invalidate the menu cache (call after database updates)
        /// </summary>
        void InvalidateCache();

        /// <summary>
        /// Check if user has access to a specific menu
        /// </summary>
        /// <param name="menuId">Menu ID to check</param>
        /// <param name="userLevel">User's role level</param>
        /// <returns>True if user can access menu</returns>
        Task<bool> UserHasAccessAsync(int menuId, int userLevel);
    }
}
