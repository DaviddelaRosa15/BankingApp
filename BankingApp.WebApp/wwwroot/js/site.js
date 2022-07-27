// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


const cliente = document.querySelector(".selectUser");
var optionSelect = cliente.options[cliente.selectedIndex].value;
if (optionSelect == "Cliente") {
    var inputHidden = document.querySelector(".inputHidden");
    var labeltHidden = document.querySelector(".labeltHidden");
    var span_Amount = document.querySelector(".span_Amount");
    inputHidden.type = "number";
    labeltHidden.removeAttribute("hidden");
    span_Amount.removeAttribute("hidden");
}

cliente.addEventListener("change", (even) => {
    if (even.target.value == "Cliente") {
        var inputHidden = document.querySelector(".inputHidden");
        var labeltHidden = document.querySelector(".labeltHidden");
        var span_Amount = document.querySelector(".span_Amount");       
        inputHidden.type = "number";
        labeltHidden.removeAttribute("hidden");
        span_Amount.removeAttribute("hidden");
    }
    else {
        var inputHidden = document.querySelector(".inputHidden");
        var labeltHidden = document.querySelector(".labeltHidden");
        var span_Amount = document.querySelector(".span_Amount");
        inputHidden.type = "hidden";
        labeltHidden.setAttribute("hidden", "hidden");
        span_Amount.setAttribute("hidden", "hidden");
    }    
    
});






