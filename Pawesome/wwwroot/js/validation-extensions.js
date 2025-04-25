$(function () {
    $.validator.addMethod("passwordComplexity", function (value, element, params) {
        return this.optional(element) ||
            (/[A-Z]/.test(value) &&
                /[a-z]/.test(value) &&
                /[0-9]/.test(value) &&
                /[^a-zA-Z0-9]/.test(value));
    }, "Le mot de passe ne respecte pas les règles de complexité");

    $.validator.unobtrusive.adapters.addBool("passwordComplexity");
});