// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

async function viewClick(element) {
    try {
        const response = await fetch(`User/GetPartialView?view=${encodeURIComponent(element.name)}`, { method: "GET" });
        const data = await response.text();
        const $container = document.getElementById("id-partial-container");
        $container.innerHTML = data;
    } catch (e) {
        console.log(e);
    }
}

function handleRegexChange() {
    const $txt = document.getElementById('id-regex-txt');    
    if(!isRegexValid($txt.value))
        return;
    const $tests = document.querySelectorAll('.test-regex');
    const $ok = document.querySelectorAll('.regex-ok');

    for (let i = 0; i < $tests.length; i++) {
        let tmp = $tests[i].textContent;
        const regex = new RegExp($txt.value);
        let isMatch = regex.test(tmp);
        let matches = tmp.match(regex);
        let result = matches === null ? "" : matches.join();
        if($txt.value===""){
            $ok[i].style.color = "black";
            $ok[i].textContent = " OK ";
        }        
        else if (isMatch && result === tmp) {
            $ok[i].style.color = "green";
            $ok[i].textContent = " OK " + result;
        }
        else if (isMatch && result !== "") {
            $ok[i].style.color = "blue";
            $ok[i].textContent = " OK " + result;
        }
        else {
            $ok[i].style.color = "red";
            $ok[i].textContent = " OK ";
        } 
    }
}

function handleOneRegexChange(idRegex, idTest, idOk) {
    const $txt = document.getElementById(idRegex);
    if (!isRegexValid($txt.value))
        return;
    const $tests = document.getElementById(idTest);
    const $ok = document.getElementById(idOk);

    let tmp = $tests.textContent;
    const regex = new RegExp($txt.value);
    let isMatch = regex.test(tmp);
    let matches = tmp.match(regex);
    let result = matches === null ? "" : matches.join();
    if ($txt.value === "") {
        $ok.style.color = "black";
        $ok.textContent = " OK ";
    }
    else if (isMatch && result === tmp) {
        $ok.style.color = "green";
        $ok.textContent = " OK " + result;
    }
    else if (isMatch && result !== "") {
        $ok.style.color = "blue";
        $ok.textContent = " OK " + result;
    }
    else {
        $ok.style.color = "red";
        $ok.textContent = " OK ";
    }
}

function isRegexValid(expresion) {
    try {
      const regex = new RegExp(expresion);
      return true;
    } catch (error) {
      return false;
    }
}

function checkTestToSubmit(testName) {
    const $tests = document.querySelectorAll(`.${testName} p`);
    for (let i = 0; i < $tests.length; i++) {
        const $test = $tests[i].querySelector(`span:nth-of-type(1)`);
        const $result = $tests[i].querySelector(`span:nth-of-type(2)`);
        let testResult = $result.textContent.substring(4);
        if ($test.textContent !== testResult) {
            return;
        }
    }

    console.log('works!!');



}

// document.getElementById('id-regex-txt').addEventListener('input', handleRegexChange);