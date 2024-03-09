export class UserService {
  userDto = {};
  baseUrl = "http://localhost:5167/api/user";

  constructor(userDto) {
    this.userDto = userDto;
  }

  async parseResponse(response) {
    if (response.ok) {
      if (response.headers.get("Content-Type").includes("application/json")) return response.json();
      else return response.text();
    } else throw response.status;
  }

  async UpdateProfile(userId, token, userDtoUpdated) {
    try {
      let response = await fetch(`${this.baseUrl}profile/update/${userId}?token=${token}`, {
        method: "GET",
        mode: "cors",
        body: JSON.parse(userDtoUpdated),
      });

      let parsedRespose = await this.parseResponse(response);
      return parsedRespose;
    } catch (e) {
      throw new Error(e);
    }
  }

  async getAllLabs(token) {
    try {
      let response = await fetch(`${this.baseUrl}/labs?token=${token}`, {
        method: "GET",
        mode: "cors",
      });

      let parsedResponse = await this.parseResponse(response);
      return parsedResponse;
    } catch (e) {
      console.log("Error in getAllLabs ", e);
      throw new Error(e);
    }
  }

  async getReservations(id, token) {
    try {
      let response = await fetch(`${this.baseUrl}/labs/reservations/${id}?token=${token}`, {
        method: "GET",
        mode: "cors",
      });

      let parsedRespose = await this.parseResponse(response);
      return parsedRespose;
    } catch (e) {
      console.log(e);
      throw new Error(e);
    }
  }

  async createReservation(reservation, token) {
    try {
      let response = await fetch(`${this.baseUrl}/labs/reservations/create?token=${token}`, {
        method: "POST",
        mode: "cors",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(reservation),
      });

      let parsedResponse = await this.parseResponse(response);
      return parsedResponse;
    } catch (e) {
      console.log("Error in createReservation ", e);
      throw e;
    }
  }

  async deleteReservation(reservationId, userId, token) {
    try {
      let response = await fetch(
        `${this.baseUrl}/labs/reservations/delete/${reservationId}?userId=${userId}&token=${token}`,
        {
          method: "DELETE",
          mode: "cors",
        }
      );

      let parsedRespose = await this.parseResponse(response);
      return parsedRespose;
    } catch (e) {
      console.log(e);
      throw new Error(e);
    }
  }
}
