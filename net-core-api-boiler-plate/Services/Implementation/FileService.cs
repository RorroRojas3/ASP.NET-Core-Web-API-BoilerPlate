using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using net_core_api_boiler_plate.Database.Repository.Interface;
using net_core_api_boiler_plate.Models.Responses;
using net_core_api_boiler_plate.Services.Interface;
using File = net_core_api_boiler_plate.Database.Tables.File;

namespace net_core_api_boiler_plate.Services.Implementation
{
    /// <summary>
    ///     FileService class with IFileService interface implementation
    /// </summary>
    public class FileService : IFileService
    {
        /// <summary>
        ///     Private variables
        /// </summary>
        private readonly IRepository<File> _fileRepository;
        private readonly IMapper _mapper;

        /// <summary>
        ///     FileService constructor with DI
        /// </summary>
        /// <param name="fileRepository"></param>
        /// <param name="mapper"></param>
        public FileService(IRepository<File> fileRepository,
                            IMapper mapper)
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
        }

        /// <summary>
        ///     Deletes file from DB based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(Guid id)
        {
            return await _fileRepository.Delete(id);
        }

        /// <summary>
        ///     Gets all files from DB
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileResponse>> GetAllFiles()
        {
            var files = await _fileRepository.GetAll();
            var response = _mapper.Map<List<FileResponse>>(files);
            return response;
        }

        /// <summary>
        ///     Gets file from DB based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<File> GetFile(Guid id)
        {
            var file = await _fileRepository.Get(id);
            return file;
        }

        /// <summary>
        ///     Creates file on DB
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<bool> PostFile(IFormFile formFile)
        {
            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                data = memoryStream.ToArray();
            }

            var file = new File
            {
                Id = Guid.NewGuid(),
                Name = formFile.FileName,
                ContentType = formFile.ContentType,
                Data = data
            };

            var result = await _fileRepository.Add(file);

            return result == null ? false : true;
        }

        /// <summary>
        ///     Updates file on DB based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="formfile"></param>
        /// <returns></returns>
        public async Task<bool> UpdateFile(Guid id, IFormFile formfile)
        {
            var deleteFile = await _fileRepository.Delete(id);

            if (!deleteFile)
            {
                return false;
            }

            var addFile = await PostFile(formfile);

            if (!addFile)
            {
                return false;
            }

            return true;
        }
    }
}