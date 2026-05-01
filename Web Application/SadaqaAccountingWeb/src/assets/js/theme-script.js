// Apply fixed theme settings (locked) - do NOT read or write localStorage for theme keys
// These values are fixed as per configuration and users are not allowed to change them.
// Color scheme follows modern UX principles: accessible contrast, visual hierarchy, and brand consistency
const FIXED_THEME = {
  theme: "light",
  sidebarTheme: "light",
  color: "brightblue",
  layout: "mini",
  topbar: "bluegradient",
  topbarcolor: "firefly",
  card: "bordered",
  size: "default",
  width: "fluid",
  loader: "enable",
  // Modern, accessible color palette with proper contrast ratios (WCAG AA compliant)
  primaryRGB: "1, 122, 220", // #0f86e7ff - Custom brand blue for primary actions
  sidebarRGB: "0, 0, 0", // Slate-800 - Deep, sophisticated sidebar for better focus
  topbarcolorRGB: "15, 23, 42" // Slate-900 - High contrast topbar for excellent readability
};

document.querySelector("html").setAttribute("data-theme", FIXED_THEME.theme);
document.querySelector("html").setAttribute("data-sidebar", FIXED_THEME.sidebarTheme);
document.querySelector("html").setAttribute("data-color", FIXED_THEME.color);
document.querySelector("html").setAttribute("data-topbar", FIXED_THEME.topbar);
document.querySelector("html").setAttribute("data-layout", FIXED_THEME.layout);
document.querySelector("html").setAttribute("data-topbarcolor", FIXED_THEME.topbarcolor);
document.querySelector("html").setAttribute("data-card", FIXED_THEME.card);
document.querySelector("html").setAttribute("data-size", FIXED_THEME.size);
document.querySelector("html").setAttribute("data-width", FIXED_THEME.width);
document.querySelector("html").setAttribute("data-loader", FIXED_THEME.loader);

// Set CSS custom properties for RGB values
const _html = document.querySelector("html");
if (_html) {
  _html.style.setProperty("--primary-rgb", FIXED_THEME.primaryRGB);
  _html.style.setProperty("--sidebar-rgb", FIXED_THEME.sidebarRGB);
  _html.style.setProperty("--topbarcolor-rgb", FIXED_THEME.topbarcolorRGB);
}

document.addEventListener("DOMContentLoaded", function () {
  // Force light mode and remove any dark mode settings
  document.documentElement.setAttribute("data-theme", "light");
  localStorage.removeItem("darkMode");

  // Hide dark mode toggle buttons if they exist
  const darkModeToggle = document.getElementById("dark-mode-toggle");
  const lightModeToggle = document.getElementById("light-mode-toggle");

  if (darkModeToggle) {
    darkModeToggle.style.display = "none";
  }
  if (lightModeToggle) {
    lightModeToggle.style.display = "none";
  }

  const themeRadios = document.querySelectorAll('input[name="theme"]');
  const sidebarRadios = document.querySelectorAll('input[name="sidebar"]');
  const colorRadios = document.querySelectorAll('input[name="color"]');
  const layoutRadios = document.querySelectorAll('input[name="LayoutTheme"]');
  const topbarRadios = document.querySelectorAll('input[name="topbar"]');
  const sidebarBgRadios = document.querySelectorAll('input[name="sidebarbg"]');
  const topbarcolorRadios = document.querySelectorAll('input[name="topbarcolor"]');
  const cardRadios = document.querySelectorAll('input[name="card"]');
  const sizeRadios = document.querySelectorAll('input[name="size"]');
  const widthRadios = document.querySelectorAll('input[name="width"]');
  const loaderRadios = document.querySelectorAll('input[name="loader"]');
  const topbarbgRadios = document.querySelectorAll('input[name="topbarbg"]');
  const resetButton = document.getElementById("resetbutton");
  const sidebarBgContainer = document.getElementById("sidebarbgContainer");
  const sidebarElement = document.querySelector(".sidebar");

  function setThemeAndSidebarTheme(
    theme,
    sidebarTheme,
    color,
    layout,
    topbar,
    topbarcolor,
    card,
    size,
    width,
    loader
  ) {
    if (!sidebarElement) {
      console.error("Sidebar element not found");
      return;
    }

    // Force light theme
    document.documentElement.setAttribute("data-theme", "light");
    document.documentElement.setAttribute("data-sidebar", sidebarTheme);
    document.documentElement.setAttribute("data-color", color);
    document.documentElement.setAttribute("data-layout", layout);
    document.documentElement.setAttribute("data-topbar", topbar);
    document.documentElement.setAttribute("data-topbarcolor", topbarcolor);
    document.documentElement.setAttribute("data-card", card);
    document.documentElement.setAttribute("data-size", size);
    document.documentElement.setAttribute("data-width", width);
    document.documentElement.setAttribute("data-loader", loader);

    //track mini-layout set or not
    layout_mini = 0;
    if (layout === "mini") {
      document.body.classList.add("mini-sidebar");
      document.body.classList.remove("menu-horizontal");
      layout_mini = 1;
    } else if (layout === "horizontal") {
      document.body.classList.add("menu-horizontal");
      document.body.classList.remove("mini-sidebar");
    } else if (layout === "horizontal-single") {
      document.body.classList.add("menu-horizontal");
      document.body.classList.remove("mini-sidebar");
    } else if (layout === "horizontal-overlay") {
      document.body.classList.add("menu-horizontal");
      document.body.classList.remove("mini-sidebar");
    } else {
      document.body.classList.remove("mini-sidebar", "menu-horizontal");
    }

    if (size === "compact") {
      document.body.classList.add("mini-sidebar");
      document.body.classList.remove("expand-menu");
      layout_mini = 1;
    } else if (size === "hoverview") {
      document.body.classList.add("expand-menu");
      if (layout_mini == 0) {
        document.body.classList.remove("mini-sidebar");
      }
    } else {
      if (layout_mini == 0) {
        document.body.classList.remove("mini-sidebar");
      }
      document.body.classList.remove("expand-menu");
    }

    if (width === "box") {
      document.body.classList.add("layout-box-mode");
      document.body.classList.add("mini-sidebar");
      layout_mini = 1;
    } else {
      if (layout_mini == 0) {
        document.body.classList.remove("mini-sidebar");
      }
      document.body.classList.remove("layout-box-mode");
    }
    if (
      (width === "box" && layout === "horizontal") ||
      (width === "box" && layout === "horizontal-overlay") ||
      (width === "box" && layout === "horizontal-single") ||
      (width === "box" && layout === "without-header")
    ) {
      document.body.classList.remove("mini-sidebar");
    }

    // Show/hide sidebar background options based on layout selection
    if (layout === "box" && sidebarBgContainer) {
      sidebarBgContainer.classList.add("show");
    } else if (sidebarBgContainer) {
      sidebarBgContainer.classList.remove("show");
    }
  }

  function handleSidebarBgChange() {
    const sidebarBg = document.querySelector('input[name="sidebarbg"]:checked')
      ? document.querySelector('input[name="sidebarbg"]:checked').value
      : null;

    if (sidebarBg) {
      document.body.setAttribute("data-sidebarbg", sidebarBg);
      localStorage.setItem("sidebarBg", sidebarBg);
    } else {
      document.body.removeAttribute("data-sidebarbg");
      localStorage.removeItem("sidebarBg");
    }
  }

  function handleTopbarBgChange() {
    const topbarbg = document.querySelector('input[name="topbarbg"]:checked')
      ? document.querySelector('input[name="topbarbg"]:checked').value
      : null;

    if (topbarbg) {
      document.body.setAttribute("data-topbarbg", topbarbg);
      localStorage.setItem("topbarbg", topbarbg);
    } else {
      document.body.removeAttribute("data-topbarbg");
      localStorage.removeItem("topbarbg");
    }
  }

  function handleInputChange() {
    // Force light theme
    const theme = "light";
    const layout = document.querySelector('input[name="LayoutTheme"]:checked').value;
    const card = document.querySelector('input[name="card"]:checked').value;
    const size = document.querySelector('input[name="size"]:checked').value;
    const width = document.querySelector('input[name="width"]:checked').value;
    const loader = document.querySelector('input[name="loader"]:checked').value;

    color = localStorage.getItem("primaryRGB");
    sidebarTheme = localStorage.getItem("sidebarRGB");
    topbar = localStorage.getItem("topbarRGB");
    topbarcolor = localStorage.getItem("topbarcolorRGB");

    if (document.querySelector('input[name="color"]:checked') != null) {
      color = document.querySelector('input[name="color"]:checked').value;
    } else {
      color = "all";
    }

    if (document.querySelector('input[name="sidebar"]:checked') != null) {
      sidebarTheme = document.querySelector('input[name="sidebar"]:checked').value;
    } else {
      sidebarTheme = "all";
    }

    if (document.querySelector('input[name="topbar"]:checked') != null) {
      topbar = document.querySelector('input[name="topbar"]:checked').value;
    } else {
      topbar = "all";
    }

    if (document.querySelector('input[name="topbarcolor"]:checked') != null) {
      topbarcolor = document.querySelector('input[name="topbarcolor"]:checked').value;
    } else {
      topbarcolor = "all";
    }

    setThemeAndSidebarTheme(
      theme,
      sidebarTheme,
      color,
      layout,
      topbar,
      topbarcolor,
      card,
      size,
      width,
      loader
    );
  }

  function resetThemeAndSidebarThemeAndColorAndBg() {
    setThemeAndSidebarTheme(
      "light",
      "light",
      "primary",
      "default",
      "white",
      "white",
      "bordered",
      "default",
      "fluid",
      "enable"
    );
    document.body.removeAttribute("data-sidebarbg");
    document.body.removeAttribute("data-topbarbg");

    document.getElementById("lightTheme").checked = true;
    document.getElementById("lightSidebar").checked = true;
    document.getElementById("primaryColor").checked = true;
    document.getElementById("defaultLayout").checked = true;
    document.getElementById("whiteTopbar").checked = true;
    document.getElementById("whiteTopbarcolor").checked = true;
    document.getElementById("borderedCard").checked = true;
    document.getElementById("defaultSize").checked = true;
    document.getElementById("fluidWidth").checked = true;
    document.getElementById("enableLoader").checked = true;

    const checkedSidebarBg = document.querySelector('input[name="sidebarbg"]:checked');
    if (checkedSidebarBg) {
      checkedSidebarBg.checked = false;
    }

    localStorage.removeItem("sidebarBg");

    const checkedTopbarBg = document.querySelector('input[name="topbarbg"]:checked');
    if (checkedTopbarBg) {
      checkedTopbarBg.checked = false;
    }

    localStorage.removeItem("topbarbg");
  }

  // Theme is locked: disable interactive controls and DO NOT attach listeners that change theme
  function disableControls(radios) {
    radios.forEach((radio) => {
      try {
        radio.disabled = true;
      } catch (e) {}
    });
  }

  disableControls(themeRadios);
  disableControls(sidebarRadios);
  disableControls(colorRadios);
  disableControls(layoutRadios);
  disableControls(topbarRadios);
  disableControls(topbarcolorRadios);
  disableControls(cardRadios);
  disableControls(sizeRadios);
  disableControls(widthRadios);
  disableControls(loaderRadios);
  disableControls(sidebarBgRadios);
  disableControls(topbarbgRadios);

  // Disable reset button as well
  if (resetButton) {
    resetButton.style.pointerEvents = "none";
    resetButton.classList.add("disabled");
  }

  // Initial setup from FIXED_THEME (no localStorage usage)
  const savedTheme = "light"; // Force light mode
  savedSidebarTheme = FIXED_THEME.sidebarTheme;
  savedColor = FIXED_THEME.color;
  const savedLayout = FIXED_THEME.layout;
  savedTopbar = FIXED_THEME.topbar;
  savedTopbarcolor = FIXED_THEME.topbarcolor;
  const savedCard = FIXED_THEME.card;
  const savedSize = FIXED_THEME.size;
  const savedWidth = FIXED_THEME.width;
  const savedLoader = FIXED_THEME.loader;
  const savedSidebarBg = null;
  const savedTopbarBg = null;

  // setup theme color all - use FIXED_THEME primaryRGB
  const savedColorPickr = FIXED_THEME.primaryRGB;
  if (savedColor == null && savedColorPickr == null) {
    savedColor = "primary";
  } else if (savedColorPickr != null && savedColor == null) {
    savedColor = "all";
    let html = document.querySelector("html");
    html.style.setProperty("--primary-rgb", savedColorPickr);
  }

  // setup theme topbar all
  const savedTopbarPickr = FIXED_THEME.topbarRGB || FIXED_THEME.topbarcolorRGB;
  if (savedTopbar == null && savedTopbarPickr == null) {
    savedTopbar = "white";
  } else if (savedTopbarPickr != null && savedTopbar == null) {
    savedTopbar = "all";
    let html = document.querySelector("html");
    html.style.setProperty("--topbar-rgb", savedTopbarPickr);
  }

  // setup theme topbarcolor all
  const savedTopbarcolorPickr = FIXED_THEME.topbarcolorRGB;
  if (savedTopbarcolor == null && savedTopbarcolorPickr == null) {
    savedTopbarcolor = "white";
  } else if (savedTopbarcolorPickr != null && savedTopbarcolor == null) {
    savedTopbarcolor = "all";
    let html = document.querySelector("html");
    html.style.setProperty("--topbarcolor-rgb", savedTopbarcolorPickr);
  }

  // setup theme color all
  const savedSidebarPickr = FIXED_THEME.sidebarRGB;
  if (savedSidebarTheme == null && savedSidebarPickr == null) {
    savedSidebarTheme = "light";
  } else if (savedSidebarPickr != null && savedSidebarTheme == null) {
    savedSidebarTheme = "all";
    let html = document.querySelector("html");
    html.style.setProperty("--sidebar-rgb", savedSidebarPickr);
  }

  setThemeAndSidebarTheme(
    savedTheme,
    savedSidebarTheme,
    savedColor,
    savedLayout,
    savedTopbar,
    savedTopbarcolor,
    savedCard,
    savedSize,
    savedWidth,
    savedLoader
  );

  // No saved sidebar/topbar backgrounds in locked mode
  document.body.removeAttribute("data-sidebarbg");
  document.body.removeAttribute("data-topbarbg");

  // Set UI controls to reflect fixed theme and keep them disabled
  function setCheckedIfExists(id) {
    const el = document.getElementById(id);
    if (el) {
      try {
        el.checked = true;
        el.disabled = true;
      } catch (e) {}
    }
  }

  setCheckedIfExists(`lightTheme`); // Force light theme
  setCheckedIfExists(`${savedSidebarTheme}Sidebar`);
  setCheckedIfExists(`${savedColor}Color`);
  setCheckedIfExists(`${savedLayout}Layout`);
  setCheckedIfExists(`${savedTopbar}Topbar`);
  setCheckedIfExists(`${savedTopbarcolor}Topbarcolor`);
  setCheckedIfExists(`${savedCard}Card`);
  setCheckedIfExists(`${savedSize}Size`);
  setCheckedIfExists(`${savedWidth}Width`);
  setCheckedIfExists(`${savedLoader}Loader`);

  // Initially hide sidebar background options based on layout
  if (savedLayout !== "box" && sidebarBgContainer) {
    sidebarBgContainer.classList.remove("show");
  }
});
