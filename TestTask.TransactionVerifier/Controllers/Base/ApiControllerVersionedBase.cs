using Microsoft.AspNetCore.Mvc;

namespace TestTask.TransactionVerifier.WebApi.Controllers.Base;

/// <summary>
/// Base versioned api controller.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ApiControllerVersionedBase : ControllerBase
{

}
