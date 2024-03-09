import { AdminService } from "../services/AdminService.js";
import { labCardComponent } from "../shared/LabCardComponent.js";

/**
 * this function is launched when the DOM loads.
 * takes care of recovering (if any) user data within the local storage.
 * if these exist the UserService class is instantiated, otherwise you are redirected to the login page
 */
function checkUser() {
  const userStorage = localStorage.getItem("user_logged");

  if (userStorage) {
    const adminService = new AdminService(JSON.parse(userStorage));
    console.log(adminService);
    return adminService;
  } else {
    window.location.href = "http://127.0.0.1:5500/views/login.html";
  }
}

function logout() {
  localStorage.clear();
  window.location.href = "http://127.0.0.1:5500/views/login.html";
}

// this function creates the table that shows all the list of computers in a given laboratory
function createComputerList(contentDiv, obj, currentLabId) {
  const containerId = `computerList-${currentLabId}`;

  /**
   * I perform a check not to create a double table if one already exists,
   * keeping inside the DOM the form prepared for the operations of deleting
   * computers added and updated (This is inside the HTML.
   * Is a pretty big bootstrap element, I didn’t want to recreate
   * it dynamically in this script)
   */
  // if (document.getElementById(containerId)) {
  //   return;
  // }

  // for (let child of contentDiv.childNodes) {
  //   if (contentDiv.querySelector("#staticBackdrop")) {
  //     continue;
  //   }
  //   contentDiv.removeChild(child);
  // }

  while (contentDiv.lastChild.id !== "staticBackdrop") {
    contentDiv.removeChild(contentDiv.lastChild);
  }

  // create the table
  const tableContainer = document.createElement("div");
  tableContainer.setAttribute("id", containerId);

  const buttonRow = document.createElement("div");
  buttonRow.classList.add("row");

  buttonRow.innerHTML += `
		<div class="col">
			<button id="createButton" data-bs-toggle="modal" data-bs-target="#staticBackdrop" class="btn btn-primary">Add Computer</button>
		</div>
	`;

  const computersTable = document.createElement("table");
  computersTable.classList.add("table");
  computersTable.classList.add("table-hover");
  computersTable.innerHTML += `
		<thead>
				<tr>
				<th scope="col">ID</th>
				<th scope="col">Description</th>
				<th scope="col">Status</th>
				<th scope="col">Lab Assigned</th>
				<th scope="col">Actions</th>
				</tr>
		</thead>
	`;

  const tableBody = document.createElement("tbody");
  for (let computer of obj) {
    const row = document.createElement("tr");
    row.innerHTML += `
			<td>${computer.id}</td>
			<td>${computer.description}</td>
			<td>${computer.status}</td>
			<td>Lab ${computer.labAssigned}</td>
			<td>
				<button id="deleteButton" type="button" class="btn btn-danger">Delete</button>
				<button id="updateButton" type="button" data-bs-toggle="modal" data-bs-target="#staticBackdrop" class="btn btn-secondary">Update</button>
			</td>
		`;
    tableBody.appendChild(row);
  }

  // reset the content of the div where the list will appear to not create duplicates
  // contentDiv.innerHTML = "";
  computersTable.appendChild(tableBody);
  tableContainer.appendChild(buttonRow);
  tableContainer.appendChild(computersTable);
  contentDiv.appendChild(tableContainer);
}

window.addEventListener("load", async function () {
  const adminService = checkUser();

  const userName = adminService.adminDto.username;
  const userNameTextContent = document.createTextNode(userName);
  const dropdownMenu = document.getElementById("my_dropdown");
  dropdownMenu.appendChild(userNameTextContent);

  const logoutButton = document.getElementById("logoutButton");
  logoutButton.addEventListener("click", logout);

  async function performActionsForDiv(divId) {
    const selectedDiv = document.getElementById(divId);

    switch (divId) {
      case "lab-list":
        try {
          const labs = await adminService.getAllLabs(adminService.adminDto.token);
          console.table(labs); // DEBUG

          labs.forEach((lab) => {
            const labCardId = `labCard-${lab.id}`;
            // function that creates the cards corresponding to the laboratories
            labCardComponent(selectedDiv, lab, labCardId);

            const cardButton = selectedDiv.querySelector(`#${labCardId} .btn-primary`);
            cardButton.textContent = "";
            cardButton.textContent = "Computer List";
            cardButton.setAttribute("href", `#computer-list`);
            cardButton.addEventListener("click", function () {
              // except the labid inside the local storage
              localStorage.setItem("selectedLab", lab.id);
            });
          });
        } catch (e) {
          console.log(e);
        }
        break;
      case "computer-list":
        /**
         * I could not create a fake routing with a dynamic path
         * ( or better, the path can be dynamic, but the switch cases accept only constants).
         * plus when the page was reloaded the variable currentLabId was lost,
         * so I saved inside the local storage this variable,
         * allowing the page update without the loss of the list
         */
        const labInStorage = localStorage.getItem("selectedLab");

        /**
         * Loading the list of computers is allowed only if there is a laboratoryId
         * inside the local storage. otherwise the user is sent back to the lab page
         * forcing him to choose one, which will be saved in local storage.
         */
        if (labInStorage) {
          const computersForm = document.getElementById("computers-form");
          const labAssignedSelect = document.getElementById("labAssigned");
          const createSubmitButton = document.getElementById("createSubmitButton");
          const updateSubmitButton = document.getElementById("updateSubmitButton");
          let computerId;

          try {
            let computers = await adminService.getComputers(
              labInStorage,
              adminService.adminDto.token
            );

            console.table(computers);

            createComputerList(selectedDiv, computers, labInStorage);

            // through these buttons will be launched the form (unique in the sun).
            //at the click of each the latter will be manipulated to conform to the action to be performed
            const createButton = document.querySelector("#createButton");
            const deleteButton = document.querySelectorAll("#deleteButton");
            const updateButton = document.querySelectorAll("#updateButton");

            createButton.addEventListener("click", function () {
              // show the button that will allow the create action
              createSubmitButton.classList.remove("d-none");
              // I’m hiding the update button.
              updateSubmitButton.classList.add("d-none");
              // hide the field that allows the assignment of the computer to another laboratory
              labAssignedSelect.classList.add("d-none");
            });

            createSubmitButton.addEventListener("click", async function () {
              const formData = new FormData(computersForm);
              const computerDescription = formData.get("description");
              const computerStatus = formData.get("status");

              const computerRequestBody = {
                description: computerDescription,
                status: new Number(computerStatus),
                labAssigned: labInStorage,
              };

              await adminService.createComputer(
                computerRequestBody,
                labInStorage,
                adminService.adminDto.token
              );
              location.reload();
            });

            updateButton.forEach(function (button, index) {
              button.addEventListener("click", function () {
                // show the update button.
                updateSubmitButton.classList.remove("d-none");
                // I'm hiding the button that will allow the create action
                createSubmitButton.classList.add("d-none");
                // show the field that allows the assignment of the computer to another laboratory
                labAssignedSelect.classList.remove("d-none");

                computerId = computers[index].id;
                console.log(computerId);
              });
            });

            updateSubmitButton.addEventListener("click", async function () {
              const formData = new FormData(computersForm);
              // formData.set();
              const computerDescription = formData.get("description");
              const computerStatus = formData.get("status");
              const labAssigned = formData.get("labAssigned");

              const computerRequestBody = {
                description: computerDescription,
                status: new Number(computerStatus),
                labAssigned: labAssigned,
              };

              await adminService.updateComputer(
                computerRequestBody,
                labInStorage,
                computerId,
                adminService.adminDto.token
              );
              location.reload();
            });

            deleteButton.forEach(function (button, index) {
              button.addEventListener("click", async function () {
                await adminService.deleteComputer(
                  labInStorage,
                  computers[index].id,
                  adminService.adminDto.token
                );
                location.reload();
              });
            });
          } catch (e) {
            console.log(e);
            throw Error("Error in computer-list ", e);
          }
        } else {
          window.location.href = "http://127.0.0.1:5500/views/adminIndex.html#lab-list";
        }
        break;
      case "profile":
        break;
    }
  }

  function updateDomElement() {
    let selectedElement = window.location.hash.substring(1);
    const divs = document.querySelectorAll("main > div");
    divs.forEach((div) => {
      div.classList.add("d-none");
    });

    let selectedDiv = document.getElementById(selectedElement);
    if (selectedDiv) {
      selectedDiv.classList.remove("d-none");
      performActionsForDiv(selectedElement);
    }
  }

  window.addEventListener("hashchange", updateDomElement);

  updateDomElement();
});
