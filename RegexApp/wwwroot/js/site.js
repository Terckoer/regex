// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.



async function viewClick(element) {
    const response = await fetch(`User/GetPartialView?view=${element.name}`, { method: "GET" });
    const data = await response.text();
    const $container = document.getElementById("id-partial-container");
    $container.innerHTML = data;
}

function handleRegexChange(testName, ok) {
    const $txt = document.getElementById('id-regex-txt');
    const $tests = document.querySelectorAll(`.${testName}`);
    const $ok = document.querySelectorAll(`.${ok}`);

    for (let i = 0; i < $tests.length; i++) {
        let tmp = $tests[i].textContent;
        const regex = new RegExp($txt.textContent);
        let isMatch = regex.test(tmp);
        if (isMatch)
            $ok[i].style.color = "green";
        else 
            $ok[i].style.color = "red";
        
    }
}