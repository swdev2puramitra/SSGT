namespace WebApp.Models
{
    /// <summary>
    /// Data Transfer Object for menu items with hierarchical structure
    /// Used to represent menus in views with parent-child relationships
    /// </summary>
    public class MenuDTO
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = null!;

        public int ParentId { get; set; }

        public byte UserLevel { get; set; }

        public int SortOrder { get; set; }

        public string? FormName { get; set; }

        public string? MenuIcon { get; set; }

        public string? MenuDescription { get; set; }

        public bool IsActive { get; set; }

        public string? MenuUrl { get; set; }

        /// <summary>
        /// Child menus (only populated for parent items)
        /// </summary>
        public List<MenuDTO>? Children { get; set; }

        /// <summary>
        /// Indicates if this menu has child menus
        /// </summary>
        public bool HasChildren => Children != null && Children.Count > 0;

        /// <summary>
        /// Gets the display URL for the menu item
        /// Rules:
        /// 1. If parent menu (HasChildren) → Return "#" (no navigation, show children via dropdown)
        /// 2. Otherwise → Return MenuUrl (already resolved by FromEntity in backend)
        ///    - If MenuUrl is empty/null → Under Construction redirect URL with menu name
        /// </summary>
        public string GetUrl()
        {
            // Parent menus don't navigate, they show children
            if (HasChildren)
            {
                return "#";
            }

            // Return the menu URL (already fully resolved by backend in FromEntity)
            return MenuUrl ?? "#";
        }

        /// <summary>
        /// Converts from FwMenu entity to MenuDTO
        /// Handles URL resolution in the backend:
        /// - If MenuUrl is provided, use it
        /// - If MenuUrl is null, generate Under Construction redirect URL
        /// Note: FormName is kept for reference but not used for URL generation
        /// </summary>
        public static MenuDTO FromEntity(FwMenu menu)
        {
            // Resolve URL in backend
            string? resolvedUrl = ResolveMenuUrl(menu.MenuUrl, menu.MenuName);

            return new MenuDTO
            {
                MenuId = menu.MenuId,
                MenuName = menu.MenuName,
                ParentId = menu.ParentId,
                UserLevel = menu.UserLevel,
                SortOrder = menu.SortOrder,
                FormName = menu.FormName,
                MenuIcon = menu.MenuIcon,
                MenuDescription = menu.MenuDescription,
                IsActive = menu.IsActive ?? false,
                MenuUrl = resolvedUrl,
                Children = null
            };
        }

        /// <summary>
        /// Resolves the menu URL based on MenuUrl only
        /// If MenuUrl is null, returns Under Construction redirect URL
        /// </summary>
        private static string? ResolveMenuUrl(string? menuUrl, string menuName)
        {
            // Use MenuUrl if provided
            if (!string.IsNullOrEmpty(menuUrl))
            {
                return menuUrl.StartsWith("/") ? menuUrl : $"/{menuUrl}";
            }

            // If MenuUrl is null, redirect to Under Construction page
            return $"/Home/UnderConstruction?menuName={System.Net.WebUtility.UrlEncode(menuName)}";
        }
    }
}
