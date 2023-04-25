// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.



async function viewClick(element) {
    const response = await fetch(`User/GetPartialView?view=${element.name}`, { method: "GET" });
    const data = await response.text();
    const $container = document.getElementById("id-partial-container");
    $container.innerHTML = data;
}
