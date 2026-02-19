using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public record IncomeDocumentDetails
    (
        string DocumentType,
        string Summary,
        string SenderNumber,
        string Sender
    );

    public record OutcomeDocumentDetails
        (
        string Summary,
        string Resipient,
        string Performer
        );

    public record ContractDetails
        (
        string ContractSubject,
        string ContractType,
        string Party1,
        string Party2,
        string Party1Name,
        string Party2Name,
        string Cost,
        string WithNDS,
        string TotalCost
        );

    public record NoteDetails
        (
        string DocumentType,
        string Topic,
        string Content,
        //string Adressees,
        string LastName,
        string FirstName
        );

    public record OrderDetails
        (
        string DocumentType,
        string OrderName,
        string OrderContent,
        //string Adressees,
        string LastName,
        string FirstName
        //DateTime SigningDate
        );

    public record GroupDetails
        (
        string GroupName,
        string Responsible,
        bool IsAdmin,
        bool IsGip,
        bool IsArchive,
        bool IsContracts,
        bool IsOrd        
        );
}
