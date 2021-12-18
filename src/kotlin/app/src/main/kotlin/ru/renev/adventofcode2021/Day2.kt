package ru.renev.adventofcode2021

class Day2 {
    fun part1(): Int {
        println("Day 2 part 1")

        val parts = Day2Input.values.split("\n")
        var pos = 0
        var depth = 0
        for (command in parts) {
            val (action, amount) = command.split(" ")
            val amountValue = amount.toInt()

            when (action) {
                "forward" -> pos += amountValue
                "down" -> depth += amountValue
                "up" -> depth -= amountValue
                else -> throw RuntimeException("Invalid action $action")
            }
        }

        return pos * depth
    }

    fun part2(): Int {
        println("Day 2 part 2")

        val parts = Day2Input.values.split("\n")
        var pos = 0
        var depth = 0
        var aim = 0
        for (command in parts) {
            val (action, amount) = command.split(" ")
            val amountValue = amount.toInt()

            when (action) {
                "forward" -> {
                    pos += amountValue
                    depth += aim * amountValue
                }
                "down" -> aim += amountValue
                "up" -> aim -= amountValue
                else -> throw RuntimeException("Invalid action $action")
            }
        }

        return pos * depth
    }
}
