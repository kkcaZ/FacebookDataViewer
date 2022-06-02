function checkScrollProgress(element) {
    console.log("Scroll");
    
    if (window.scrollInfoService != null)
        window.scrollInfoService.invokeMethodAsync("OnScroll", element.scrollTop, element.scrollHeight);
}

window.RegisterScrollInfoService = (scrollInfoService) => {
    window.scrollInfoService = scrollInfoService;
}

function openFilter() {
    var filterBar = document.getElementById("filterBar");
    
    if (filterBar.classList.contains("shown")) {
        filterBar.classList.remove("shown");
        document.getElementById("filterArrow").innerText = "\\/";
    }
    else {
        filterBar.classList.add("shown");
        document.getElementById("filterArrow").innerText = "/\\";
    }
}