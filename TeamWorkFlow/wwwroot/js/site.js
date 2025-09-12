// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var message = (function () {
  // Check if toastr is available before using it
  if (typeof toastr === "undefined") {
    console.warn("Toastr library is not loaded");
    return {
      success: function (msg) {
        console.log("Success:", msg);
      },
      error: function (msg) {
        console.error("Error:", msg);
      },
      warning: function (msg) {
        console.warn("Warning:", msg);
      },
      info: function (msg) {
        console.info("Info:", msg);
      },
    };
  }

  toastr.options = {
    closeButton: false,
    debug: false,
    newestOnTop: false,
    progressBar: false,
    positionClass: "toast-top-right",
    preventDuplicates: false,
    onclick: null,
    showDuration: "300",
    hideDuration: "1000",
    timeOut: "5000",
    extendedTimeOut: "1000",
    showEasing: "swing",
    hideEasing: "linear",
    showMethod: "fadeIn",
    hideMethod: "fadeOut",
  };
  var showSuccess = function (message) {
    if (typeof toastr !== "undefined") {
      toastr["success"](message);
    } else {
      console.log("Success:", message);
    }
  };

  var showError = function (message) {
    if (typeof toastr !== "undefined") {
      toastr["error"](message);
    } else {
      console.error("Error:", message);
    }
  };

  return {
    showSuccess,
    showError,
  };
})();
