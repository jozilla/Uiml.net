using System;


class Dictionary
{
	public static string Lookup(String animal)
	{
		switch(animal)
		{
			case "Dog":
				return "Domestic animal related to a wolf that's fond of chasing cats";
			case "Cat":
				return "Carnivourous, domesticated mammal that's fond of rats and mice";
			case "Mouse":
				return "Small rodent often seen running away from a cat";
			default:
				return "Unknown animal";

		}
	}
}
