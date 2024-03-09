export class LoginService {
   baseUrl = "http://localhost:5167/api/login";

   async parseResponse(response) {
      if (response.ok) {
         if (response.headers.get("Content-Type").includes("application/json"))
            return response.json();
         else return response.text();
      } else throw new Error(response.text);
   }

   async GetAuthentication(username) {
      try {
         let response = await fetch(`${this.baseUrl}/${username}`, {
            method: "GET",
            mode: "cors",
         });

         let parsedRespose = await this.parseResponse(response);
         return parsedRespose;
      } catch (e) {
         console.log(e);
         throw new Error("Invalid credentials");
      }
   }

   async Authentication(username, challenge) {
      try {
         let response = await fetch(
            `${this.baseUrl}/${username}/${challenge}`,
            {
               method: "GET",
               mode: "cors",
            }
         );

         let parsedRespose = await this.parseResponse(response);
         return parsedRespose;
      } catch (e) {
         console.log(e);
         throw new Error("Invalid credentials");
      }
   }
}
