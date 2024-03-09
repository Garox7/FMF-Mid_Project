export class AdminService {
  adminDto = {};
  baseUrl = "http://localhost:5167/api/admin";

  constructor(adminDto) {
    this.adminDto = adminDto;
  }

  async parseResponse(response) {
    if (response.ok) {
      if (response.headers.get("Content-Type").includes("application/json")) return response.json();
      else return response.text();
    } else throw response.status;
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
      throw e;
    }
  }

  async getComputers(labId, token) {
    try {
      let response = await fetch(`${this.baseUrl}/labs/${labId}/computers?token=${token}`, {
        method: "GET",
        mode: "cors",
      });

      let parsedResponse = await this.parseResponse(response);
      return parsedResponse;
    } catch (e) {
      console.log("Error in GetComputers ", e);
      throw e;
    }
  }

  async createComputer(computerDto, labId, token) {
    try {
      let response = await fetch(`${this.baseUrl}/labs/${labId}/computers/create?token=${token}`, {
        method: "POST",
        mode: "cors",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(computerDto),
      });

      let parsedResponse = await this.parseResponse(response);
      return parsedResponse;
    } catch (e) {
      console.log(e);
      throw e;
    }
  }

  async updateComputer(computerDto, labId, computerId, token) {
    try {
      let response = await fetch(
        `${this.baseUrl}/labs/${labId}/computers/update/${computerId}?token=${token}`,
        {
          method: "PATCH",
          mode: "cors",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(computerDto),
        }
      );

      let parsedResponse = await this.parseResponse(response);
      return parsedResponse;
    } catch (e) {
      console.log(e);
      throw e;
    }
  }

  async deleteComputer(labId, computerId, token) {
    // [Route("labs/{labId}/computers/delete/{computerId}")]
    try {
      let response = await fetch(
        `${this.baseUrl}/labs/${labId}/computers/delete/${computerId}?token=${token}`,
        {
          method: "DELETE",
          mode: "cors",
        }
      );

      let parsedRespose = this.parseResponse(response);
      return parsedRespose;
    } catch (e) {
      console.log(e);
      throw e;
    }
  }
}
