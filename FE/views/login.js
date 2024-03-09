import { LoginService } from "../Services/LoginService.js";

function invalidCredentialError() {
  const invalidFeedback = document.querySelector(".invalid-credentials");
  invalidFeedback.classList.remove("hidden");
}

window.addEventListener("load", function () {
  const form = document.getElementById("login-form");
  const submitButton = document.getElementById("submitButton");

  submitButton.addEventListener("click", async function (event) {
    event.preventDefault();
    const formData = new FormData(form);
    const username = formData.get("username");
    const psw = formData.get("password");

    const loginService = new LoginService();
    let challenge;

    try {
      await loginService.GetAuthentication(username).then((response) => {
        challenge = response;
        console.log(challenge); // DEBUG
      });

      await loginService.Authentication(username, `${psw + challenge}`).then((response) => {
        console.log(response); // DEBUG

        const userString = JSON.stringify(response);
        localStorage.setItem("user_logged", userString);

        if (response.userRole == 1) {
          window.location.href = "http://127.0.0.1:5500/views/adminIndex.html#lab-list";
        } else if (response.userRole == 0) {
          window.location.href = "http://127.0.0.1:5500/views/userIndex.html#prenotations";
        }
      });
    } catch (e) {
      if (e instanceof TypeError) {
        console.log(e);
      }

      invalidCredentialError();
      return;
    }
  });
});
