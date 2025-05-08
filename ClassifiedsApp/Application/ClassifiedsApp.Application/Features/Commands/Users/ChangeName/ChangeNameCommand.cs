using ClassifiedsApp.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Features.Commands.Users.ChangeName;

public class ChangeNameCommand : IRequest<Result>
{
	public string Name { get; set; }
}
