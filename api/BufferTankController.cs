using Microsoft.AspNetCore.Mvc;
using OpcUaRefresher.Models;
using OpcUaRefresher.services;

[ApiController]
[Route("api/telemetry/buffer-tank")]
public class BufferTankController(IBufferTankService bufferTankService) : ControllerBase
{

    [HttpGet("liquid-level")]
    public ActionResult<BufferTank> GetBufferTankLiquidLevel()
    {
        return Ok(bufferTankService.GetLiquidLevelSensor());
    }
}