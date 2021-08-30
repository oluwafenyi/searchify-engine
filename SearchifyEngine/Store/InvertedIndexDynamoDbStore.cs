using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SearchifyEngine.Indexer;

namespace SearchifyEngine.Store
{
    
    /// <summary>
    /// Inverted Index Store for DynamoDB. Provides methods for CRU operations on the Inverted Index in DynamoDB
    /// </summary>
    public class InvertedIndexDynamoDbStore: IStore
    {
        private AmazonDynamoDBClient _client;

        /// <summary>
        /// Instantiates a new InvertedIndexDynamoDbStore object
        /// </summary>
        /// <param name="client">dynamodb client</param>
        public InvertedIndexDynamoDbStore(AmazonDynamoDBClient client)
        {
            _client = client;
        }
        
        /// <summary>
        /// Returns the id of last file indexed, zero if no file was indexed.
        /// </summary>
        /// <returns>id of last file indexed</returns>
        public async Task<uint> GetLastId()
        {
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                ["Term"] = new AttributeValue { S = "last.id" }
            };

            var request = new GetItemRequest { TableName = "inverted_index", Key = key };

            GetItemResponse response;
            response = await _client.GetItemAsync(request);
            Dictionary<string, AttributeValue> item = response.Item;
            
            try
            {
                return UInt32.Parse(item["id"].N);
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
        }
        
        /// <summary>
        /// Sets the value of the last document indexed
        /// </summary>
        /// <param name="lastId">document id</param>
        /// <returns>status code for operation</returns>
        public async Task<HttpStatusCode> SetLastId(uint lastId)
        {
            Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>
            {
                ["Term"] = new AttributeValue { S = "last.id" },
                ["id"] = new AttributeValue { N = lastId.ToString() }
            };

            var request = new PutItemRequest
            {
                TableName = "inverted_index",
                Item = attributes
            };

            var response = await _client.PutItemAsync(request);
            return response.HttpStatusCode;
        }

        /// <summary>
        /// Checks if a term has been indexed
        /// </summary>
        /// <param name="term">term</param>
        /// <returns>true if term has been indexed, else false</returns>
        public async Task<bool> CheckTermIndexed(string term)
        {
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                ["Term"] = new AttributeValue { S = term }
            };

            var request = new GetItemRequest { TableName = "inverted_index", Key = key };

            try
            {
                var response = await _client.GetItemAsync(request);
                Dictionary<string, AttributeValue> item = response.Item;
                return item["TermList"].L != null;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Appends to list of index terms for a particular term. If the term has not been indexed yet, a new list is
        /// instantiated and the term is then appended
        /// </summary>
        /// <param name="term">term</param>
        /// <param name="indexTerm"><see cref="IndexTerm"/> object</param>
        /// <returns>status code of operation</returns>
         public async Task<HttpStatusCode> AppendIndexTerm(string term, IndexTerm indexTerm)
         {
             List<AttributeValue> positionsAttrs = new List<AttributeValue>();
            
            foreach (var indexTermPosition in indexTerm.Positions)
            {
                positionsAttrs.Add(new AttributeValue{ N = indexTermPosition.ToString() });
            }

            if (!await CheckTermIndexed(term))
            {
                Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>
                {
                    ["Term"] = new AttributeValue { S = term },
                    ["TermList"] = new AttributeValue
                    {
                        L = new List<AttributeValue>
                        {
                            new AttributeValue // indexTerm
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    ["fileDelta"] = new AttributeValue{ N = indexTerm.FileDelta.ToString() },
                                    ["positions"] = new AttributeValue{ L = positionsAttrs }
                                }
                            }
                        }
                    }
                };
                
                PutItemRequest request = new PutItemRequest
                {
                    TableName = "inverted_index",
                    Item = attributes
                };

                var response = await _client.PutItemAsync(request);
                
                return response.HttpStatusCode;
            }
            else
            {
                Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
                {
                    ["Term"] = new AttributeValue { S = term }
                };

                Dictionary<string, AttributeValueUpdate> updates = new Dictionary<string, AttributeValueUpdate> 
                {
                    ["TermList"] = new AttributeValueUpdate
                    {
                        Action = AttributeAction.ADD,
                        Value = new AttributeValue
                        {
                            L = new List<AttributeValue>
                            {
                                new AttributeValue
                                {
                                    M = new Dictionary<string, AttributeValue>
                                    {
                                        ["fileDelta"] = new AttributeValue{ N = indexTerm.FileDelta.ToString() },
                                        ["positions"] = new AttributeValue{ L = positionsAttrs }
                                    }
                                }
                            }
                        }
                    }
                };

                UpdateItemRequest request = new UpdateItemRequest
                {
                    TableName = "inverted_index",
                    Key = key,
                    AttributeUpdates = updates
                };

                var response = await _client.UpdateItemAsync(request);
                
                return response.HttpStatusCode;
            }
         }

        /// <summary>
        /// Returns index term list for a particular term. An empty list is returned if the term has not been indexed
        /// </summary>
        /// <param name="term">term</param>
        /// <returns>list of <see cref="IndexTerm"/> objects</returns>
        public async Task<List<IndexTerm>> GetIndexTermList(string term)
        {
            List<IndexTerm> indexTerms = new List<IndexTerm>();
            
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                ["Term"] = new AttributeValue { S = term }
            };

            var request = new GetItemRequest { TableName = "inverted_index", Key = key };

            try
            {
                var response = await _client.GetItemAsync(request);
                Dictionary<string, AttributeValue> item = response.Item;
                foreach (var attributeValue in item["TermList"].L)
                {
                    Dictionary<string, AttributeValue> indexTermMap = attributeValue.M;
                    uint fileDelta = uint.Parse(indexTermMap["fileDelta"].N);
                    List<uint> positions = new List<uint>();
                    foreach (var value in indexTermMap["positions"].L)
                    {
                        positions.Add(uint.Parse(value.N));
                    }
                    IndexTerm indexTerm = new IndexTerm(fileDelta);
                    indexTerm.AddPositions(positions.ToArray());
                    indexTerms.Add(indexTerm);
                }
            }
            catch (KeyNotFoundException)
            {
            }
            return indexTerms;
        }
    }
}