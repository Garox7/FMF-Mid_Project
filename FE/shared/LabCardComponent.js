export function labCardComponent(showIn, lab, cardId) {
  /**
   * Inside the section div > #showIn
   *
   * In case "USER"
   *  - there is a modal containing the form for booking a slot time
   *    for that particular laboratory (will be assigned the first available pc, if existing).
   *
   * In case "ADMIN"
   *  - there is a modal that contains the form for creating a new computer
   *
   * through this control we make sure that every time the section is reloaded
   * existing cards are not recreated if they exist
   */
  if (showIn.querySelector(`#${cardId}`)) {
    return;
  }

  const cols = document.createElement("div");
  let colsClass = ["col-12", "col-sm-6", "col-md-4", "col-lg-3"];
  cols.classList.add(...colsClass);
  cols.setAttribute("id", cardId);

  const labCard = document.createElement("div");
  labCard.classList.add("card");

  const labCardBody = document.createElement("div");
  labCardBody.classList.add("card-body");

  const cardTitle = document.createElement("h5");
  cardTitle.classList.add("card-title");
  const cardTitleInnerText = document.createTextNode(`${lab.name}`);
  cardTitle.appendChild(cardTitleInnerText);

  const cardText = document.createElement("p");
  cardText.classList.add("card-text");
  const cardTextInnerText = document.createTextNode(`${lab.description}`);
  cardText.appendChild(cardTextInnerText);

  const cardButton = document.createElement("a");
  cardButton.classList.add("btn");
  cardButton.classList.add("btn-primary");

  cols.appendChild(labCard);
  labCard.appendChild(labCardBody);
  labCardBody.appendChild(cardTitle);
  labCardBody.appendChild(cardText);
  labCardBody.appendChild(cardButton);
  showIn.appendChild(cols);
}
