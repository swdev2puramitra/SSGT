using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FireWorks.UI
{
    /// <summary>
    /// Small runtime helper that applies UI theming to any Form that is opened during application lifetime.
    /// This avoids editing every form's constructor. It runs on a short timer and applies UiHelpers.ApplyFormStyle()
    /// once per form.
    /// </summary>
    internal static class UiAutoStyler
    {
        private static Timer _timer;
        private static HashSet<IntPtr> _styledHandles = new HashSet<IntPtr>();
        private static Icon s_appIcon;
        private static readonly object s_lock = new object();
        private static bool s_initialized;

        public static Icon AppIcon
        {
            get
            {
                if (s_appIcon != null) return s_appIcon;

                lock (s_lock)
                {
                    if (s_appIcon != null) return s_appIcon;

                    try
                    {
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favicon.ico");
                        if (File.Exists(path))
                        {
                            s_appIcon = new Icon(path);
                        }
                    }
                    catch { }

                    if (s_appIcon == null)
                    {
                        try
                        {
                            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                            var resourceName = asm.GetManifestResourceNames()
                                .FirstOrDefault(n => n.EndsWith("favicon.ico", StringComparison.OrdinalIgnoreCase));
                            if (resourceName != null)
                            {
                                using (var stream = asm.GetManifestResourceStream(resourceName))
                                {
                                    if (stream != null)
                                        s_appIcon = new Icon(stream);
                                }
                            }
                        }
                        catch { }
                    }

                    return s_appIcon;
                }
            }
        }

        // Call once at application startup (e.g. before Application.Run(...))
        public static void InitializeAppIcon()
        {
            if (s_initialized) return;
            s_initialized = true;

            // Apply to already-open forms (none at Main start, but safe)
            try
            {
                if (AppIcon != null)
                {
                    foreach (Form f in Application.OpenForms)
                    {
                        try
                        {
                            if (!IsIconOptOut(f))
                                f.Icon = AppIcon;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Install a message filter to set icon for forms created later
            Application.AddMessageFilter(new _FormCreateIconInstaller());
        }

        private static bool IsIconOptOut(Form f)
        {
            try
            {
                return f.Tag != null && f.Tag.ToString() == "__no_icon";
            }
            catch { return false; }
        }

        private class _FormCreateIconInstaller : IMessageFilter
        {
            // WM_CREATE = 0x0001
            private const int WM_CREATE = 0x0001;

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_CREATE && AppIcon != null)
                {
                    try
                    {
                        var ctl = Control.FromHandle(m.HWnd);
                        if (ctl is Form form)
                        {
                            // Opt-out support using Tag = "__no_icon"
                            if (IsIconOptOut(form))
                                return false;

                            // Always set the icon to the application icon (override designer icons)
                            try { form.Icon = AppIcon; } catch { }
                        }
                    }
                    catch { }
                }

                return false; // don't block message
            }
        }

        public static void Initialize()
        {
            if (_timer != null) return;
            _timer = new Timer();
            _timer.Interval = 500; // 0.5s - lightweight periodic check
            _timer.Tick += (s, e) =>
            {
                try
                {
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f == null) continue;

                        // Skip forms that opt out via Tag
                        if (IsIconOptOut(f)) continue;

                        IntPtr h = f.Handle;
                        if (_styledHandles.Contains(h)) continue;
                        try
                        {
                            // Apply form style non-destructively
                            UiHelpers.ApplyFormStyle(f);

                            // Ensure the app icon is applied even if the designer set another icon
                            try
                            {
                                if (AppIcon != null)
                                    f.Icon = AppIcon;
                            }
                            catch { }

                            _styledHandles.Add(h);
                        }
                        catch { }
                    }
                }
                catch { }
            };
            _timer.Start();
        }
    }
}