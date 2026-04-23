using MyTextApi.Models.DTOs;

namespace MyTextApi.Interfaces;

public interface IWorkerService
{
    Task<List<WorkerResponse>> GetAllWorkersAsync();
    Task<List<WorkerResponse>?> GetWorkersByUserIdAsync(int userId);
    Task<WorkerResponse?> GetWorkerByIdAsync(int id);
    Task<WorkerResponse> CreateWorkerAsync(CreateWorkerRequest request);
    Task<WorkerResponse?> UpdateWorkerAsync(int id, CreateWorkerRequest request);
    Task<bool> DeleteWorkerById(int id);
}