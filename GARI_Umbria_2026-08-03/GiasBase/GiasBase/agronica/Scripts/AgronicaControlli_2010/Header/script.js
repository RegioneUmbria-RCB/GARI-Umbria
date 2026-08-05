/**
 * Defines
 * @widthFromLeftContent {string} define width and marginleft fors content after sidenav open
 */
const widthFromLeftContent = '350px';
const widthFromLeftContentMobile = '100%';
const SIDE_MENU_TABS_KEY = 'gias-sidemenu-selected-tab';
var widthLeftSidebar = (window.innerWidth < 991) ? widthFromLeftContentMobile : widthFromLeftContent;

/**
 * This function will open the sidenav menu
 */
function openNav() {
    document.getElementById("GiasSidenav").style.width = widthLeftSidebar;
    document.getElementById("openMenu").classList.add("hide")
    document.getElementById("closeMenu").classList.remove("hide");
    document.getElementById("GiasSidenav").classList.add("openSidebarMenu");
    document.getElementById("searchService").focus();
    
}

/* Set the width of the side navigation to 0 and the left margin of the page content to 0 */
/**
 * This function will close the sidenav menu
 */
function closeNav() {
    document.getElementById("GiasSidenav").style.width = "0";
    document.getElementById("openMenu").classList.remove("hide")
    document.getElementById("closeMenu").classList.add("hide");
    document.getElementById("GiasSidenav").classList.remove("openSidebarMenu");
}

/**
 * function to toggle show/hide dropdown element on header section
 * @param id
 */
function showToggleDropdown(id) {
    var currentElement = $('#'+id).clone();
    $(".dropdown-content").each(function (i, el) {
        $(this).removeClass("show");
    });
    if (!currentElement.hasClass("show"))
        $('#' + id).addClass("show");

}

/**
 * implements tabs located in sidenav header;
 * optimized for this situation
 * @param target the clicked tab name
 */
function openLeftsideTab(target) {
    
    let targetTabId = "leftsideTabServizi";
    let currentTabId = "leftsideTabPreferiti";
    let targetTabcontentId = "leftsideTabcontentServizi";
    let currentTabcontentId = "leftsideTabcontentPreferiti";
    if (target === "preferiti") {
        currentTabId = "leftsideTabServizi";
        targetTabId = "leftsideTabPreferiti";
        currentTabcontentId = "leftsideTabcontentServizi";
        targetTabcontentId = "leftsideTabcontentPreferiti";
    }
    const targetTab = document.getElementById(targetTabId);
    targetTab.classList.add("active");
    const currentTab = document.getElementById(currentTabId);
    currentTab.classList.remove("active");

    const targetTabcontent = document.getElementById(targetTabcontentId);
    targetTabcontent.classList.remove("hide");
    const currentTabcontent = document.getElementById(currentTabcontentId);
    currentTabcontent.classList.add("hide");

    $(`#${targetTabcontentId} input`).focus();
    let figli = document.querySelectorAll(`#${targetTabcontentId} > .nano-content > .sub-menu`);
    if (figli.length == 1 && (["none", ""].includes(figli[0].querySelector("ul").style.display))) { //Se c'e' un solo figlio E non e' gia aperto, allora lo apro.
        figli[0].querySelector("a").click();
    }

    localStorage.setItem(SIDE_MENU_TABS_KEY, target);
}
