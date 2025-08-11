export function saveFile(name, content, contentType) {
    // Create the URL
    const file = new File([content], name, { type: contentType });
    const exportUrl = URL.createObjectURL(file);

    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = name;
    a.target = "_self";
    a.click();

    URL.revokeObjectURL(exportUrl);
    document.body.removeChild(a);

    return true;
}