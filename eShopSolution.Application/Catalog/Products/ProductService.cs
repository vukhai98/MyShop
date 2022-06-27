using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities.Exceptions;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public class ProductService : IProductService
    {
        private readonly EShopDbContext _context;

        private readonly IStorageService _storageService;

        public ProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            product.ViewCount += 1;

            await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoAlias = request.SeoDescription,
                        SeoTitle = request.SeoTitle,
                        SeoDescription = request.SeoDescription,
                        LanguageId = request.LanguageId
                    }
                }
            };

            // Save image 
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated= DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product.Id;
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new EShopException($"Cannot find a product: {productId}");
            }

            var images = _context.ProductImages.Where(x => x.Id == productId);

            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }

            _context.Products.Remove(product);

            return await _context.SaveChangesAsync();

        }

        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
        {
            // 1 Select join

            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            // 2. Filter

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            }

            if (request.CategoryIds.Count > 0)
            {
                query = query.Where(p => request.CategoryIds.Contains(p.pic.CategoryId));
            }

            // 3. Pagging 

            var totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                            .Take(request.PageSize)
                            .Select(x => new ProductViewModel()
                            {
                                Id = x.p.Id,
                                Name = x.pt.Name,
                                Price = x.p.Price,
                                OriginalPrice = x.p.OriginalPrice,
                                DateCreated = x.p.DateCreated,
                                SeoAlias = x.pt.SeoAlias,
                                SeoDescription = x.pt.SeoDescription,
                                SeoTitle = x.pt.SeoTitle,
                                LanguageId = x.pt.LanguageId,
                                Stock = x.p.Stock,
                                ViewCount = x.p.ViewCount,
                                Description = x.pt.Description,
                                Details = x.pt.Details
                            }).ToListAsync();

            // 4. Select and projection

            var pagedResult = new PagedResult<ProductViewModel>()
            {
                Items = data,
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize

            };

            return pagedResult;

        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslations = await _context.ProductTranslations
                                                    .FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId);

            if (productTranslations == null || product == null)
            {
                throw new EShopException($"Cannot find a product with id: {request.Id}");
            }

            productTranslations.Description = request.Description;
            productTranslations.SeoDescription = request.SeoDescription;
            productTranslations.Name = request.Name;
            productTranslations.Details = request.Details;
            productTranslations.SeoAlias = request.SeoAlias;
            productTranslations.SeoTitle = request.SeoTitle;

            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.IsDefault == true && x.ProductId == request.Id);

                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _context.ProductImages.Update(thumbnailImage);
                }
            }

            return await _context.SaveChangesAsync();
        }


        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                throw new EShopException($"Cannot find a product with id: {productId}");

            product.Price = newPrice;

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                throw new EShopException($"Cannot find a product with id: {productId}");

            product.Stock += addedQuantity;

            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);

            return fileName;
        }

        public async Task<int> RemoveImage(int imageId)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);

            if (productImage == null)
            {
                throw new EShopException($"Cannot find a product with imageId: {imageId}");
            }

            _context.ProductImages.Remove(productImage);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<ProductImageViewModel>> GetListImage(int productId)
        {
            return await _context.ProductImages.Where(x => x.ProductId == productId)
                                               .Select(i => new ProductImageViewModel()
                                               {
                                                   Caption = i.Caption,
                                                   DateCreated = i.DateCreated,
                                                   FileSize = i.FileSize,
                                                   Id = i.Id,
                                                   ImagePath = i.ImagePath,
                                                   IsDefault = i.IsDefault,
                                                   ProductId = i.ProductId,
                                                   SortOrder = i.SortOrder

                                               }).ToListAsync();
        }

        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = productId,
                SortOrder = request.SortOrder

            };

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }

            await _context.ProductImages.AddAsync(productImage);

            await _context.SaveChangesAsync();

            return productImage.Id;
        }

        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);

            if (productImage == null)
                throw new EShopException($"Cannot find an image with id {imageId}");

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Update(productImage);

            return await _context.SaveChangesAsync();
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);

            if (image == null)
                throw new EShopException($"Cannot find an image with id {imageId}");

            var imageViewModel = new ProductImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                Id = image.Id,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };

            return imageViewModel;

        }

        public async Task<ProductViewModel> GetProductById(int productId, string languageId)
        {
            var product = await _context.Products.FindAsync(productId);
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId && x.LanguageId == languageId);

            var productViewModel = new ProductViewModel()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                ViewCount = product.ViewCount,
                Stock = product.Stock,
                Description = productTranslation != null ? productTranslation.Description : null,
                Details = productTranslation != null ? productTranslation.Details : null,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                LanguageId = productTranslation != null ? productTranslation.LanguageId : null,
                Name = productTranslation != null ? productTranslation.Name : null,

            };

            return productViewModel;
        }

        public async Task<PagedResult<ProductViewModel>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request)
        {
            // 1 Select join

            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };

            // 2. Filter

            if (request.CategoryId.Value > 0 && request.CategoryId.HasValue)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }

            // 3. Pagging 

            var totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                            .Take(request.PageSize)
                            .Select(x => new ProductViewModel()
                            {
                                Id = x.p.Id,
                                Name = x.pt.Name,
                                Price = x.p.Price,
                                OriginalPrice = x.p.OriginalPrice,
                                DateCreated = x.p.DateCreated,
                                SeoAlias = x.pt.SeoAlias,
                                SeoDescription = x.pt.SeoDescription,
                                SeoTitle = x.pt.SeoTitle,
                                LanguageId = x.pt.LanguageId,
                                Stock = x.p.Stock,
                                ViewCount = x.p.ViewCount,
                                Description = x.pt.Description,
                                Details = x.pt.Details
                            }).ToListAsync();

            // 4. Select and projection

            var pagedResult = new PagedResult<ProductViewModel>()
            {
                Items = data,
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return pagedResult;

        }
    }
}
