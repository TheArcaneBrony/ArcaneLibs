function BlazorFocusElement(element) {
    if (element == null) return;
    if (element instanceof HTMLElement) {
        console.log(element);
        element.focus();
    } else if (element.hasOwnProperty("__internalId")) {
        console.log("Element is not an HTMLElement", element);
    }
}