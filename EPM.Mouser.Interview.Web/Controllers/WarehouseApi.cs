using EPM.Mouser.Interview.Data;
using EPM.Mouser.Interview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EPM.Mouser.Interview.Web.Controllers
{
    [Route("api/warehouse/")]
    public class WarehouseApi : Controller
    {
        private readonly IWarehouseRepository _warehouseRepo;
        
        public WarehouseApi(IWarehouseRepository warehouseRepository)
        {
            this._warehouseRepo = warehouseRepository ?? throw new ArgumentNullException(nameof(warehouseRepository));
        }

        /*
         *  Action: GET
         *  Url: api/warehouse/id
         *  This action should return a single product for an Id
         */
        [HttpGet("{id}")]
        public async Task<Product?> GetProduct(long id)
        {
            Product? product = await _warehouseRepo.Get(id);
            if (product == null)
            {
                //Return Empty Product???
                product = new Product();
            }
           
            return product;
        }

        /*
         *  Action: GET
         *  Url: api/warehouse
         *  This action should return a collection of products in stock
         *  In stock means In Stock Quantity is greater than zero and In Stock Quantity is greater than the Reserved Quantity
         */
        [HttpGet]
        public async Task<List<Product>> GetPublicInStockProducts()
        {
            List<Product> products = await _warehouseRepo.List();                
            return products.Where(p => p.InStockQuantity > 0 && p.InStockQuantity > p.ReservedQuantity).ToList();
        }

        /*
         *  Action: GET
         *  Url: api/warehouse/order
         *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
         *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
         *       {
         *           "id": 1,
         *           "quantity": 1
         *       }
         *
         *  This action should increase the Reserved Quantity for the product requested by the amount requested
         *
         *  This action should return failure (success = false) when:
         *     - ErrorReason.NotEnoughQuantity when: The quantity being requested would increase the Reserved Quantity to be greater than the In Stock Quantity.
         *     - ErrorReason.QuantityInvalid when: A negative number was requested
         *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        [HttpPut("order")]
        public async Task<UpdateResponse> OrderItem([FromBody]UpdateQuantityRequest updateQuantityRequest)
        {
            UpdateResponse updateResponse = new UpdateResponse();

            Product? product = await _warehouseRepo.Get(updateQuantityRequest.Id);
            if (product != null)
            {
                if ((product.ReservedQuantity + updateQuantityRequest.Quantity) > product.InStockQuantity)
                {
                    updateResponse.ErrorReason = ErrorReason.NotEnoughQuantity;
                }
                else if (updateQuantityRequest.Quantity < 0)
                {
                    updateResponse.ErrorReason = ErrorReason.QuantityInvalid;
                }
                else
                {
                    product.ReservedQuantity += updateQuantityRequest.Quantity;
                    await _warehouseRepo.UpdateQuantities(product);
                    updateResponse.Success = true;
                }
            }
            else
            {
                updateResponse.ErrorReason = ErrorReason.InvalidRequest;
            }

            return updateResponse;
        }

        /*
         *  Url: api/warehouse/ship
         *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
         *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
         *       {
         *           "id": 1,
         *           "quantity": 1
         *       }
         *
         *
         *  This action should:
         *     - decrease the Reserved Quantity for the product requested by the amount requested to a minimum of zero.
         *     - decrease the In Stock Quantity for the product requested by the amount requested
         *
         *  This action should return failure (success = false) when:
         *     - ErrorReason.NotEnoughQuantity when: The quantity being requested would cause the In Stock Quantity to go below zero.
         *     - ErrorReason.QuantityInvalid when: A negative number was requested
         *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        [HttpPut("ship")]
        public async Task<UpdateResponse> ShipItem([FromBody]UpdateQuantityRequest updateQuantityRequest)
        {
            UpdateResponse updateResponse = new UpdateResponse();

            Product? product = await _warehouseRepo.Get(updateQuantityRequest.Id);
            if (product != null)
            {
                if ((product.InStockQuantity - updateQuantityRequest.Quantity) < 0)
                {
                    updateResponse.ErrorReason = ErrorReason.NotEnoughQuantity;
                }
                else if (updateQuantityRequest.Quantity < 0)
                {
                    updateResponse.ErrorReason = ErrorReason.QuantityInvalid;
                }
                else
                {
                    if ((product.ReservedQuantity - updateQuantityRequest.Quantity) < 0)
                    {
                        product.ReservedQuantity = 0;
                    } 
                    else 
                    { 
                        product.ReservedQuantity -= updateQuantityRequest.Quantity;
                    }
                    product.InStockQuantity -= updateQuantityRequest.Quantity;
                    await _warehouseRepo.UpdateQuantities(product);
                    updateResponse.Success = true;
                }
            }
            else
            {
                updateResponse.ErrorReason = ErrorReason.InvalidRequest;
            }

            return updateResponse;
        }

        /*
        *  Url: api/warehouse/restock
        *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
        *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
        *       {
        *           "id": 1,
        *           "quantity": 1
        *       }
        *
        *
        *  This action should:
        *     - increase the In Stock Quantity for the product requested by the amount requested
        *
        *  This action should return failure (success = false) when:
        *     - ErrorReason.QuantityInvalid when: A negative number was requested
        *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        [HttpPut("restock")]
        public async Task<UpdateResponse> RestockItem([FromBody]UpdateQuantityRequest updateQuantityRequest)
        {
            UpdateResponse updateResponse = new UpdateResponse();                      

            Product? product = await _warehouseRepo.Get(updateQuantityRequest.Id);
            if (product != null)
            {
                if (updateQuantityRequest.Quantity < 0)
                {
                    updateResponse.ErrorReason = ErrorReason.QuantityInvalid;
                }
                else
                {
                    product.InStockQuantity += updateQuantityRequest.Quantity;
                    await _warehouseRepo.UpdateQuantities(product);
                    updateResponse.Success = true;
                }
            }
            else
            {
                updateResponse.ErrorReason = ErrorReason.InvalidRequest;
            }

            return updateResponse;
        }

        /*
        *  Url: api/warehouse/add
        *  This action should return a EPM.Mouser.Interview.Models.CreateResponse<EPM.Mouser.Interview.Models.Product>
        *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.Product in JSON format in the body of the request
        *       {
        *           "id": 1,
        *           "inStockQuantity": 1,
        *           "reservedQuantity": 1,
        *           "name": "product name"
        *       }
        *
        *
        *  This action should:
        *     - create a new product with:
        *          - The requested name - But forced to be unique - see below
        *          - The requested In Stock Quantity
        *          - The Reserved Quantity should be zero
        *
        *       UNIQUE Name requirements
        *          - No two products can have the same name
        *          - Names should have no leading or trailing whitespace before checking for uniqueness
        *          - If a new name is not unique then append "(x)" to the name [like windows file system does, where x is the next avaiable number]
        *
        *
        *  This action should return failure (success = false) and an empty Model property when:
        *     - ErrorReason.QuantityInvalid when: A negative number was requested for the In Stock Quantity
        *     - ErrorReason.InvalidRequest when: A blank or empty name is requested
        */
        [HttpPost("add")]
        public async Task<CreateResponse<Product>> AddNewProduct([FromBody]Product newProduct)
        {
            CreateResponse<Product> createResponse = new CreateResponse<Product>();
                        
            if (newProduct != null)
            {
                if (newProduct.InStockQuantity < 0)
                {
                    createResponse.ErrorReason = ErrorReason.QuantityInvalid;
                }
                else if (newProduct.Name == null || newProduct.Name == string.Empty)
                {
                    createResponse.ErrorReason = ErrorReason.InvalidRequest;
                }
                else
                {
                    //Remove leading + trailing spaces
                    newProduct.Name = newProduct.Name.Trim();
                    List<Product> products = await _warehouseRepo.List();

                    //Check Duplicate Product Name
                    if (products.Where(p => p.Name == newProduct.Name).Count() > 0)
                    {
                        //Check for Next Available Number - assume only up to 10 check
                        for (int i = 1; i < 10; i++)
                        {
                            string newProductName = newProduct.Name + " " + "(" + i + ")";
                            if (products.Where(p => p.Name == newProductName).Count() == 0)
                            {
                                newProduct.Name = newProductName;
                                break;
                            }                            
                        }                        
                    }
                    createResponse.Model = await _warehouseRepo.Insert(newProduct);
                    createResponse.Success = true;
                }
            }

            return createResponse;
        }
    }
}
