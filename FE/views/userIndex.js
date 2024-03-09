import { UserService } from "../services/UserService.js";
import { labCardComponent } from "../shared/LabCardComponent.js";

/**
 * this function is launched when the DOM loads.
 * takes care of recovering (if any) user data within the local storage.
 * if these exist the UserService class is instantiated, otherwise you are redirected to the login page
 */
function checkUser() {
  const userStorage = localStorage.getItem("user_logged");

  if (userStorage) {
    const userService = new UserService(JSON.parse(userStorage));
    return userService;
  } else {
    window.location.href = "http://127.0.0.1:5500/views/login.html";
  }
}

function logout() {
  localStorage.clear();
  window.location.href = "http://127.0.0.1:5500/views/login.html";
}

function displayErrorPopup(showIn, errorMessage) {
  const errorPopup = document.createElement("div");
  errorPopup.classList.add("error-popup");

  const errorTitle = document.createElement("h6");
  errorTitle.innerHTML = "Error: ";

  const errorText = document.createElement("p");
  errorText.innerHTML = errorMessage;

  errorPopup.appendChild(errorTitle);
  errorPopup.appendChild(errorText);

  showIn.appendChild(errorPopup);
}

window.addEventListener("load", async function () {
  const userService = checkUser();

  // I insert the username in the dropdown menu in the header of the page
  const userName = userService.userDto.username;
  const userNameTextContent = document.createTextNode(userName);
  const dropdownMenu = document.getElementById("my_dropdown");
  dropdownMenu.appendChild(userNameTextContent);

  // add the logout function to the logout button of the dropdown menu
  const logoutButton = document.getElementById("logoutButton");
  logoutButton.addEventListener("click", logout);

  async function performActionsForDiv(divId) {
    const selectedDiv = document.getElementById(divId);
    switch (divId) {
      case "prenotations":
        try {
          let prenotations = await userService.getReservations(
            userService.userDto.id,
            userService.userDto.token
          );
          console.table(prenotations); // DEBUG

          const prenotationTable = document.createElement("table");
          prenotationTable.classList.add("table");
          prenotationTable.classList.add("table-hover");
          prenotationTable.innerHTML += `
            <thead>
                <tr>
                <th scope="col">Lab</th>
                <th scope="col">Computer</th>
                <th scope="col">Day</th>
                <th scope="col">Time slot</th>
                <th scope="col">Actions</th>
                </tr>
            </thead>
          `;

          const tableBody = document.createElement("tbody");

          for (let prenotation of prenotations) {
            const row = document.createElement("tr");
            row.innerHTML += `
              <td>${prenotation.labId}</td>
              <td>${prenotation.computerId}</td>
              <td>${new Date(prenotation.date).toLocaleDateString("en-EN", {
                day: "numeric",
                month: "long",
                year: "numeric",
              })}
              </td>
              <td>${prenotation.timeSlot}:00 - ${prenotation.timeSlot + 1}:00</td>
              <td><button type="button" id="deleteButton" class="btn btn-danger">Delete</button></td>
          `;

            tableBody.appendChild(row);
          }

          selectedDiv.innerHTML = "";
          prenotationTable.appendChild(tableBody);
          selectedDiv.appendChild(prenotationTable);

          const deleteButtons = document.querySelectorAll("#deleteButton");

          deleteButtons.forEach(function (button, index) {
            button.addEventListener("click", async function () {
              await userService.deleteReservation(
                prenotations[index].id,
                userService.userDto.id,
                userService.userDto.token
              );
              location.reload();
            });
          });
        } catch (e) {
          console.log(e);
          displayErrorPopup(selectedDiv, e.message);
        }
        break;
      case "newPrenotation":
        const newReservationForm = document.getElementById("reservation-form");
        const submitButton = document.getElementById("submitButton");
        let currentLabId;

        try {
          let labs = await userService.getAllLabs(userService.userDto.token);
          console.table(labs); //DEBUG

          labs.forEach((lab) => {
            const labCardId = `labCard-${lab.id}`;
            // function that creates the cards corresponding to the laboratories
            labCardComponent(selectedDiv, lab, labCardId);

            const cardButton = selectedDiv.querySelector(`#${labCardId} .btn-primary`);
            cardButton.setAttribute("data-bs-toggle", "modal");
            cardButton.setAttribute("data-bs-target", "#staticBackdrop");
            cardButton.textContent = "";
            cardButton.textContent = "Book in this Lab";
            cardButton.addEventListener("click", function () {
              currentLabId = lab.id;
            });
          });

          submitButton.addEventListener("click", async function () {
            const formData = new FormData(newReservationForm);
            const dateTime = formData.get("date");
            const slot = formData.get("time-slot");

            console.log("Lab ID:", currentLabId); // DEBUG
            const reservationRequestBody = {
              labId: currentLabId,
              date: dateTime,
              timeSlot: slot,
              userId: userService.userDto.id,
            };

            await userService.createReservation(reservationRequestBody, userService.userDto.token);
            window.location.href = "http://127.0.0.1:5500/views/userIndex.html#prenotations";
          });
        } catch (e) {
          console.log(e);
          displayErrorPopup(selectedDiv, e);
        }
        break;
      case "profile":
        const userNameSurname = document.getElementById("userNameSurname");
        const userRole = document.getElementById("userRole");
        const userName = document.getElementById("Username");
        const hoursOfUse = document.getElementById("hoursOfUse");

        userNameSurname.textContent = "";
        userRole.textContent = "";
        userName.textContent = "";
        hoursOfUse.textContent = "";

        userNameSurname.textContent = `${userService.userDto.name} ${userService.userDto.surname}`;
        userRole.textContent = userService.userDto.role = 0 ? "User" : "Admin";
        userName.textContent = userService.userDto.username;
        hoursOfUse.textContent = userService.userDto.hoursOfUse;
        break;
    }
  }

  // this function takes care of updating the elements of the dom based on the hash of the url
  function updateDomElement() {
    // fetches the hash substring of the url ("substring" because I don’t consider the character "#")
    let selectedElement = window.location.hash.substring(1);

    // imposed by default all sections not visible with the bootstrap class "d-none"
    const divs = document.querySelectorAll("main > div");
    divs.forEach((div) => {
      div.classList.add("d-none");
    });

    // every div inside the dom has as id just the hash substring of the url.
    // in this way when the hash is updated this code will remove the "d-none" to the element you want to show
    let selectedDiv = document.getElementById(selectedElement);
    if (selectedDiv) {
      selectedDiv.classList.remove("d-none");

      // the recovered id is passed to this function which will handle the dom and show the affected sections
      performActionsForDiv(selectedElement);
    }
  }

  // the function "updateDomElement()" will be activated with every change of the hash in the url
  window.addEventListener("hashchange", updateDomElement);

  // by default on first load
  updateDomElement();
});
